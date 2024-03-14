using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	public class SwitchCommand(string name) : BaseCommand
	{
		public override string Name => name.ToLower();

		public override string Description => $"switches the active module to {Name}";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce == null)
			{
				return;
			}

			BaseModule? module = BaseModule.RegisteredModules.FirstOrDefault(x => x.Name.ToLower().Equals(Name, StringComparison.CurrentCultureIgnoreCase));
			if (module == null)
			{
				logSerivce.LogError($"Unknown module requested. Requested: {Name}");
			}

			BaseModule.ActiveModule = module;
		}
	}
}