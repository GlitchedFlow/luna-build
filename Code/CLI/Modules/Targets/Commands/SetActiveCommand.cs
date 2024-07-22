// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using Luna.Core.Target;

namespace Luna.CLI.Modules.Targets
{
	/// <summary>
	/// Set active target command for the CLI.
	/// </summary>
	public class SetActiveCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "set";

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

			if (args.Length == 0)
			{
				logSerivce.LogError($"Parameter missing. Please provide the name of the target.");
				return false;
			}

			int targetCount = ServiceProvider.RegistryService.GetTargetCount();
			for (int curTargetIndex = 0; curTargetIndex < targetCount; ++curTargetIndex)
			{
				ITarget? target = ServiceProvider.RegistryService.GetTargetAt(curTargetIndex);
				if (target != null && target.Name.ToLower().Equals(args[0].ToLower(), StringComparison.CurrentCultureIgnoreCase))
				{
					logSerivce.Log($"Set active target to {(target.Name)} [was {(generatorService.ActiveTarget == null ? "[None]" : generatorService.ActiveTarget.Name)}]");
					generatorService.ActiveTarget = target;

					return true;
				}
			}

			logSerivce.LogError($"Unknown target.");

			return false;
		}
	}
}