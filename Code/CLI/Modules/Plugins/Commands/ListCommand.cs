using Luna.Core;

namespace Luna.CLI.Modules.Plugins
{
	public class ListCommand : BaseCommand
	{
		public override string Name => "list";

		public override string Description => "List all requested plugins";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce != null)
			{
				logSerivce.Log($"== Requested Plugins [Count: {LunaConfig.Instance.Plugins.Count}] == ");
				using LogScope scope = new();

				foreach (string plugin in LunaConfig.Instance.Plugins)
				{
					logSerivce.Log(plugin);
				}
			}
		}
	}
}