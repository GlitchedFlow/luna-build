// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Options
{
	/// <summary>
	/// Option set command for the CLI.
	/// </summary>
	public class SetOptionCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "set";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Sets an option to on/off";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			IOptionService? optionService = ServiceProvider.OptionService;
			ILogService? logSerivce = ServiceProvider.LogService;

			if (logSerivce == null)
			{
				return false;
			}

			if (args.Length != 2)
			{
				logSerivce.LogError($"Parameter missing. Expecting: [category.]option on/off");
				return false;
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
				return false;
			}

			return true;
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