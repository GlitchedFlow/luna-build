using Luna.Core;
using Luna.Core.Target;

namespace Luna.CLI.Modules.Targets
{
	public class ListCommand : BaseCommand
	{
		public override string Name => "list";

		public override string Description => "List all requested targets";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce != null)
			{
				int targetCount = ServiceProvider.RegistryService.GetTargetCount();

				logSerivce.Log($"== Available Targets [Count: {targetCount}] == ");
				using LogScope scope = new();

				for (int curTargetIndex = 0; curTargetIndex < targetCount; ++curTargetIndex)
				{
					ITarget? target = ServiceProvider.RegistryService.GetTargetAt(curTargetIndex);
					if (target != null)
					{
						logSerivce.Log(target.Name);
					}
				}
			}
		}
	}
}