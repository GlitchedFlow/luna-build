using Luna.Core;
using System;
using System.Reflection;

namespace Luna.CLI
{
	public abstract class BaseModule(string name, List<BaseCommand> commands)
	{
		private static BaseModule? _previousModule = null;
		private static BaseModule? _activeModule = null;

		public static BaseModule? ActiveModule
		{
			get => _activeModule;
			set
			{
				_previousModule = _activeModule;
				_activeModule = value;
			}
		}

		public static List<BaseModule> RegisteredModules { get; } = [];

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
		}

		public virtual void Exit()
		{
			ActiveModule = _previousModule;
		}

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

		public void HandleCommand(string command, string[] args)
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
					ServiceProvider.LogService.LogError($"{command} is an not a registered command. Current Module: {Name}");
					Help();

					return;
				}

				foundCommand.Execute(args);
			}
		}

		public string Name => name;

		public List<BaseCommand> Commands => commands;
	}
}