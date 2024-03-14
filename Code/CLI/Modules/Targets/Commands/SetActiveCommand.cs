using Luna.Core;
using Luna.Core.Target;

namespace Luna.CLI.Modules.Targets
{
	public class SetActiveCommand : BaseCommand
	{
		public override string Name => "set";

		public override string Description => "Gets the active target";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			IGeneratorService? generatorService = ServiceProvider.GeneratorService;

			if (logSerivce != null && generatorService != null)
			{
				if (args.Length == 0)
				{
					logSerivce.LogError($"Parameter missing. Please provide the name of the target.");
					return;
				}

				int targetCount = ServiceProvider.RegistryService.GetTargetCount();
				for (int curTargetIndex = 0; curTargetIndex < targetCount; ++curTargetIndex)
				{
					ITarget? target = ServiceProvider.RegistryService.GetTargetAt(curTargetIndex);
					if (target != null && target.Name.ToLower().Equals(args[0].ToLower(), StringComparison.CurrentCultureIgnoreCase))
					{
						logSerivce.Log($"Set active target to {(target.Name)} [was {(generatorService.ActiveTarget == null ? "[None]" : generatorService.ActiveTarget.Name)}]");
						generatorService.ActiveTarget = target;

						return;
					}
				}

				logSerivce.LogError($"Unknown target.");
			}
		}
	}
}