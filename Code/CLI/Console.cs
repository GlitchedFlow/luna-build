// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using System.Diagnostics.CodeAnalysis;

namespace Luna.CLI
{
	/// <summary>
	/// CLI host console.
	/// </summary>
	public class Console
	{
		private static string? _previousScript = null;
		private static string? _activeScript = null;

		/// <summary>
		/// Gets or sets the active script file.
		/// </summary>
		public static string? ActiveScript
		{
			get => _activeScript;
			set
			{
				_previousScript = _activeScript;
				_activeScript = value;
			}
		}

		/// <summary>
		/// Main entry point.
		/// </summary>
		/// <param name="args">consoel arguments</param>
		/// <returns>0 if no errors, otherwise -1.</returns>
		[RequiresAssemblyFiles()]
		[RequiresUnreferencedCode("Required to find all CLI modules.")]
		public static int Main(string[] args)
		{
			Kickstart.InitializeCoreServices();

			ILogService? log = ServiceProvider.LogService;

			if (!ArgumentParser.Instance.Parse(args))
			{
				System.Console.ReadKey();
				return -1;
			}

			BaseModule.InitModules();

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
		public static bool HandleScript(string scriptPath)
		{
			if (!File.Exists(scriptPath) || Path.GetExtension(scriptPath) != ArgumentParser.ScriptExtension)
			{
				ServiceProvider.LogService?.LogWarning("Invalid script file called.");
				return false;
			}

			ActiveScript = scriptPath;

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
						ServiceProvider.LogService?.LogWarning($"Script Warning - #exec expects a valid path to *.lusc file");
						continue;
					}

					string filePath = Path.IsPathFullyQualified(splitLine[1]) ? splitLine[1] : Path.Combine(Path.GetDirectoryName(scriptPath) ?? "", splitLine[1]);
					if (!HandleScript(filePath))
					{
						return false;
					}

					continue;
				}

				if (!Execute(line))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Maps the input to command and args.
		/// </summary>
		/// <param name="input">Raw input string</param>
		/// <returns>split input</returns>
		public static string[] HandleInput(string input)
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

		/// <summary>
		/// Executes the input.
		/// </summary>
		/// <param name="input">Input which should be executed.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public static bool Execute(string input)
		{
			if (BaseModule.ActiveModule == null)
			{
				return false;
			}

			string[] splitInput = HandleInput(input.Trim());
			string[] scopeSplit = splitInput[0].Split('.');

			if (scopeSplit.Length > 1)
			{
				if (!BaseModule.ActiveModule.HandleCommand(scopeSplit[0], [])) // Execute module switch.
				{
					return false;
				}

				if (!BaseModule.ActiveModule.HandleCommand(scopeSplit[1], splitInput[1..])) // Actual command.
				{
					BaseModule.ActiveModule.Exit(); // Execute scope.
					return false;
				}

				BaseModule.ActiveModule.Exit(); // Execute scope.
			}
			else
			{
				if (!BaseModule.ActiveModule.HandleCommand(splitInput[0], splitInput[1..]))
				{
					return false;
				}
			}

			return true;
		}
	}
}