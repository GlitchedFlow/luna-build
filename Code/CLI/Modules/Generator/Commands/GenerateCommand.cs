// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Generator
{
	/// <summary>
	/// Generate command for the CLI.
	/// </summary>
	public class GenerateCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "generate";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Generates the solution";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			IGeneratorService? generatorService = ServiceProvider.GeneratorService;
			if (logSerivce == null || generatorService == null)
			{
				return false;
			}

			if (generatorService.ActiveTarget == null)
			{
				logSerivce.LogError($"No active target was set.");
				return false;
			}

			if (!generatorService.Generate())
			{
				logSerivce.LogError("Solution was not generated.");
				return false;
			}
			else
			{
				logSerivce.LogSuccess("Solution was generated.");
				return true;
			}
		}
	}
}