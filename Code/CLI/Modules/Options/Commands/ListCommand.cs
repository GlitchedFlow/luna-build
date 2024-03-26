using Luna.Core;

namespace Luna.CLI.Modules.Options
{
	/// <summary>
	/// Options list command for the CLI.
	/// </summary>
	public class ListCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "list";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "List all options";

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

			logSerivce.Log($"== Registered Options [Count: {optionService?.GetOptionsCount()}] == ");
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

			return true;
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