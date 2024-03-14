using Luna.CLI.Modules.Core;
using Luna.Core;
using Luna.Core.Target;
using System.Diagnostics.CodeAnalysis;

namespace Luna.CLI
{
	/// <summary>
	/// CLI host console.
	/// </summary>
	public class Console
	{
		/// <summary>
		/// Main entry point.
		/// </summary>
		/// <param name="args">consoel arguments</param>
		/// <returns>0 if no errors, otherwise -1.</returns>
		[RequiresAssemblyFiles()]
		public static int Main(string[] args)
		{
			Kickstart.InitializeCoreServices();

			ILogService? log = ServiceProvider.LogService;

			if (!ArgumentParser.Instance.Parse(args))
			{
				System.Console.ReadKey();
				return -1;
			}

			if (!LunaConfig.Load(ArgumentParser.Instance.ConfigPath))
			{
				System.Console.ReadKey();
				return -1;
			}

			if (!ArgumentParser.Instance.NoCompile)
			{
				if (!Compiler.Compile())
				{
					System.Console.ReadKey();
					return -1;
				}
			}

			Kickstart.InitializeTargets();
			Kickstart.InitializePlugins();
			Kickstart.InitializeBridge();

			IGeneratorService? generatorService = ServiceProvider.GeneratorService;
			if (generatorService == null)
			{
				log?.LogError($"Generator Service is unavailable.");
				System.Console.ReadKey();
				return -1;
			}

			int targetCount = ServiceProvider.RegistryService.GetTargetCount();
			if (targetCount <= 0)
			{
				log?.LogError($"No targets available.");
				System.Console.ReadKey();
				return -1;
			}

			BaseModule.InitModules();

			BaseModule.ActiveModule = new Module();

			if (!string.IsNullOrWhiteSpace(ArgumentParser.Instance.ScriptPath))
			{
				HandleScript(ArgumentParser.Instance.ScriptPath);
			}
			else
			{
				log?.Log($"Please enter a command that should be executed.");
				log?.Log($"Enter \"help\" to get help for the current module.");
				log?.Log($"Enter \"exit\" to exit the current module.");

				while (BaseModule.ActiveModule != null)
				{
					System.Console.Write($"[{BaseModule.ActiveModule.Name}] ");
					string? input = System.Console.ReadLine();
					if (input == null)
					{
						BaseModule.ActiveModule.Help();
						continue;
					}

					Execute(input);
				}
			}

			return 0;
		}

		/// <summary>
		/// Executes a luna script.
		/// </summary>
		/// <param name="scriptPath">Path to the script file.</param>
		private static void HandleScript(string scriptPath)
		{
			string[] scriptContent = File.ReadAllLines(scriptPath);

			foreach (string line in scriptContent)
			{
				if (line.StartsWith("//"))
				{
					// Comment
					continue;
				}

				if (string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
				{
					// Empty line
					continue;
				}

				if (line.StartsWith("#exec"))
				{
					// Execute sub script.
					string[] splitLine = HandleInput(line);
					if (splitLine.Length != 2)
					{
						ServiceProvider.LogService.LogWarning($"Script Warning - #exec expects a valid path to *.lusc file");
						continue;
					}

					string filePath = splitLine[1];
					if (Path.Exists(filePath) && File.Exists(filePath) && Path.GetExtension(filePath) == ArgumentParser.ScriptExtension)
					{
						HandleScript(filePath);
					}
				}

				Execute(line);
			}
		}

		/// <summary>
		/// Maps the input to command and args.
		/// </summary>
		/// <param name="input">Raw input string</param>
		/// <returns>split input</returns>
		private static string[] HandleInput(string input)
		{
			List<string> result = [];

			string currentValue = "";
			bool isInQuotes = false;

			foreach (char c in input)
			{
				if (c == '"')
				{
					if (isInQuotes)
					{
						result.Add(currentValue);
						currentValue = "";
						isInQuotes = false;
					}
					else
					{
						isInQuotes = true;
					}
					continue;
				}
				else if (c == ' ' && !isInQuotes && !string.IsNullOrWhiteSpace(currentValue))
				{
					result.Add(currentValue);
					currentValue = "";
					continue;
				}

				currentValue += c;
			}

			if (currentValue != "")
			{
				result.Add(currentValue);
			}

			return [.. result];
		}

		private static void Execute(string input)
		{
			if (BaseModule.ActiveModule == null)
			{
				return;
			}

			string[] splitInput = HandleInput(input.Trim());
			string[] scopeSplit = splitInput[0].Split('.');

			if (scopeSplit.Length > 1)
			{
				BaseModule.ActiveModule.HandleCommand(scopeSplit[0], []); // Execute module switch.
				BaseModule.ActiveModule.HandleCommand(scopeSplit[1], splitInput[1..]); // Actual command.
				BaseModule.ActiveModule.Exit(); // Execute scope.
			}
			else
			{
				BaseModule.ActiveModule.HandleCommand(splitInput[0], splitInput[1..]);
			}
		}
	}
}