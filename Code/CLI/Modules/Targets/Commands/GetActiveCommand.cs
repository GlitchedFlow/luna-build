// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Targets
{
	/// <summary>
	/// Get active target command for the CLI.
	/// </summary>
	public class GetActiveCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "get";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Gets the active target";

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

			logSerivce.Log($"Active Target is set to {(generatorService.ActiveTarget != null ? generatorService.ActiveTarget.Name : "[None]")}");

			return true;
		}
	}
}