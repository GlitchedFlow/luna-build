using Luna.Core;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Luna.CLI
{
	/// <summary>
	/// Base class for all CLI modules.
	/// </summary>
	/// <param name="name">Name of the module.</param>
	/// <param name="commands">List of all commands for the module.</param>
	public abstract class BaseModule(string name, List<BaseCommand> commands)
	{
		private static BaseModule? _previousModule = null;
		private static BaseModule? _activeModule = null;

		/// <summary>
		/// Gets or sets the active module.
		/// </summary>
		public static BaseModule? ActiveModule
		{
			get => _activeModule;
			set
			{
				_previousModule = _activeModule;
				_activeModule = value;
			}
		}

		/// <summary>
		/// Gets a list of all registered modules.
		/// </summary>
		public static List<BaseModule> RegisteredModules { get; } = [];

		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		public string Name => name;

		/// <summary>
		/// Gets a list of all commands for this module.
		/// </summary>
		public List<BaseCommand> Commands => commands;

		/// <summary>
		/// Initializes all modules.
		/// </summary>
		[RequiresUnreferencedCode("Scans assembly to find all CLI modules.")]
		public static void InitModules()
		{
			Assembly? assembly = Assembly.GetAssembly(typeof(BaseModule));
			if (assembly == null)
			{
				return;
			}

			foreach (Type type in assembly.GetTypes())
			{
				ConstructorInfo? defaultConstructor = type.GetConstructor([]);
				if (type.IsAssignableTo(typeof(BaseModule)) && defaultConstructor != null && defaultConstructor.IsPublic)
				{
					BaseModule instance = (BaseModule)defaultConstructor.Invoke([]);
					RegisteredModules.Add(instance);
				}
			}

			string cliPluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"CLI\\{Kickstart.PluginsDir}");
			if (Directory.Exists(cliPluginsDir))
			{
				Kickstart.ScanDirAndLoadTypes(new(cliPluginsDir), null, (BaseModule module) =>
				{
					BaseModule? matchedModule = RegisteredModules.FirstOrDefault(x => x.Name == module.Name);
					if (matchedModule == null)
					{
						RegisteredModules.Add(module);
					}
					else
					{
						foreach (BaseCommand command in module.Commands)
						{
							if (matchedModule.Commands.FirstOrDefault(x => x.Name == command.Name) == null)
							{
								matchedModule.Commands.Add(command);
							}
							else
							{
								ServiceProvider.LogService?.LogWarning($"Skipping {command.Name}. There is already command with that name in {matchedModule.Name}.");
							}
						}
					}
				});
			}

			ActiveModule = new Modules.Core.Module();
		}

		/// <summary>
		/// Exits the active module.
		/// </summary>
		public virtual void Exit()
		{
			ActiveModule = _previousModule;
		}

		/// <summary>
		/// Prints help for all commands in this module.
		/// </summary>
		public void Help()
		{
			ILogService? logService = ServiceProvider.LogService;
			if (logService != null)
			{
				logService.Log($"Module {Name}:");
				using (LogScope scope = new())
				{
					foreach (BaseCommand command in commands)
					{
						command.Help();
					}
				}
			}
		}

		/// <summary>
		/// Handles a command for this module.
		/// </summary>
		/// <param name="command">Command name</param>
		/// <param name="args">Arguments for this command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public bool HandleCommand(string command, string[] args)
		{
			if (command.ToLower().Equals("help", StringComparison.CurrentCultureIgnoreCase))
			{
				Help();
			}
			else if (command.ToLower().Equals("exit", StringComparison.CurrentCultureIgnoreCase))
			{
				Exit();
			}
			else
			{
				BaseCommand? foundCommand = Commands.FirstOrDefault(x => x.Name.Equals(command, StringComparison.CurrentCultureIgnoreCase));
				if (foundCommand == null)
				{
					ServiceProvider.LogService?.LogError($"{command} is an not a registered command. Current Module: {Name}");
					Help();

					return false;
				}

				if (!foundCommand.Execute(args))
				{
					return false;
				}
			}

			return true;
		}
	}
}