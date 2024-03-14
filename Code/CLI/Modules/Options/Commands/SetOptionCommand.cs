using Luna.Core;

namespace Luna.CLI.Modules.Options
{
	public class SetOptionCommand : BaseCommand
	{
		public override string Name => "set";

		public override string Description => "Sets an option to on/off";

		public override void Execute(string[] args)
		{
			IOptionService? optionService = ServiceProvider.OptionService;
			ILogService? logSerivce = ServiceProvider.LogService;

			if (logSerivce != null)
			{
				if (args.Length != 2)
				{
					logSerivce.LogError($"Parameter missing. Expecting: [category.]option on/off");
					return;
				}

				var optionSplitted = args[0].ToLower().Split('.');
				bool hasCategory = optionSplitted.Length == 2;

				bool wasSet = false;

				optionService?.VisitGroupedOptions(x =>
				{
					if (hasCategory && x.Key != null && x.Key.ToLower().Equals(optionSplitted[0], StringComparison.CurrentCultureIgnoreCase))
					{
						wasSet = VisitGroup(x, logSerivce, optionSplitted[1], args[1].ToLower().Equals("on", StringComparison.CurrentCultureIgnoreCase));
					}
					else if (!hasCategory && string.IsNullOrWhiteSpace(x.Key))
					{
						wasSet = VisitGroup(x, logSerivce, args[0].ToLower(), args[1].ToLower().Equals("on", StringComparison.CurrentCultureIgnoreCase));
					}

					return true;
				});

				if (!wasSet)
				{
					logSerivce.LogError($"Unknown option");
				}
			}
		}

		private static bool VisitGroup(IGrouping<string, IOption> x, ILogService logSerivce, string optionName, bool isEnabled)
		{
			foreach (IOption option in x)
			{
				IFlagOption? flagOption = (IFlagOption?)option;
				if (flagOption != null && flagOption.Name.ToLower().Equals(optionName, StringComparison.CurrentCultureIgnoreCase))
				{
					logSerivce.Log($"Set {flagOption.Name} to {(isEnabled ? "ON" : "OFF")} [was {(flagOption.IsEnabled ? "ON" : "OFF")}]");
					flagOption.IsEnabled = isEnabled;
					return true;
				}
			}

			logSerivce.LogError($"Option: {optionName} not found in {x.Key}");
			return false;
		}
	}
}