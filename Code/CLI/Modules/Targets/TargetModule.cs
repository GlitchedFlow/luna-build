namespace Luna.CLI.Modules.Targets
{
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