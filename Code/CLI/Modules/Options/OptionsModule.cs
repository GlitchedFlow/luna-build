namespace Luna.CLI.Modules.Options
{
	/// <summary>
	/// Options module for the CLI.
	/// </summary>
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