using Luna.Core;

namespace Luna.CLI.Modules.Options
{
	public class ListCommand : BaseCommand
	{
		public override string Name => "list";

		public override string Description => "List all options";

		public override void Execute(string[] args)
		{
			IOptionService? optionService = ServiceProvider.OptionService;
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce != null)
			{
				logSerivce.Log($"== Registered Options [Count: {BaseModule.RegisteredModules.Count}] == ");
				using LogScope scope = new();

				optionService?.VisitGroupedOptions(x =>
				{
					if (!string.IsNullOrEmpty(x.Key))
					{
						logSerivce.Log(x.Key);

						using LogScope groupScope = new();

						VisitGroup(x, logSerivce);
					}
					else
					{
						VisitGroup(x, logSerivce);
					}
					return true;
				});
			}
		}

		private static void VisitGroup(IGrouping<string, IOption> x, ILogService logSerivce)
		{
			foreach (IOption option in x)
			{
				IFlagOption? flagOption = (IFlagOption?)option;
				if (flagOption != null)
				{
					logSerivce.Log($"{flagOption.Name} - {(flagOption.IsEnabled ? "ON" : "OFF")}");
					continue;
				}

				IValueOption? valueOption = (IValueOption?)option;
				if (valueOption != null)
				{
					logSerivce.Log($"{valueOption.Name} - {valueOption.Value}");
				}
			}
		}
	}
}