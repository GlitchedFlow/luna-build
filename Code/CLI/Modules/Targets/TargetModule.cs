namespace Luna.CLI.Modules.Targets
{
	/// <summary>
	/// Targets module for the CLI.
	/// </summary>
	public class Module : BaseModule
	{
		public Module() : base("Targets", [
			new ListCommand(),
			new GetActiveCommand(),
			new SetActiveCommand()
		])
		{
		}
	}
}