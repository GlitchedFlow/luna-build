namespace Luna.CLI.Modules.Plugins
{
	public class Module : BaseModule
	{
		public Module() : base("Plugins", [
			new ListCommand()
		])
		{
		}
	}
}