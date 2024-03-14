using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	public class ListCommand : BaseCommand
	{
		public override string Name => "list";

		public override string Description => "List all modules";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce != null)
			{
				logSerivce.Log($"== Registered Modules [Count: {BaseModule.RegisteredModules.Count}] == ");
				foreach (BaseModule module in BaseModule.RegisteredModules)
				{
					module.Help();
				}
			}
		}
	}
}