namespace Luna.CLI.Modules.Options
{
	public class Module : BaseModule
	{
		public Module() : base("Options", [
			new ListCommand(),
			new SetOptionCommand()
		])
		{
		}
	}
}