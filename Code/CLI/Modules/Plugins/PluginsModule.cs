namespace Luna.CLI.Modules.Plugins
{
	/// <summary>
	/// Plugins module for the CLI.
	/// </summary>
	public class Module : BaseModule
	{
		public Module() : base("Plugins", [
			new ListCommand()
		])
		{
		}
	}
}