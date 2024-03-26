using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Switch command for the CLI.
	/// </summary>
	/// <param name="name">Name of the module to switch to.</param>
	public class SwitchCommand(string name) : BaseCommand
	{
		/// <summary>
		/// Gets the name of the commmand.
		/// </summary>
		public override string Name => name.ToLower();

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => $"Switches the active module to {Name}";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce == null)
			{
				return false;
			}

			BaseModule? module = BaseModule.RegisteredModules.FirstOrDefault(x => x.Name.ToLower().Equals(Name, StringComparison.CurrentCultureIgnoreCase));
			if (module == null)
			{
				logSerivce.LogError($"Unknown module requested. Requested: {Name}");
				return false;
			}

			BaseModule.ActiveModule = module;
			return true;
		}
	}
}