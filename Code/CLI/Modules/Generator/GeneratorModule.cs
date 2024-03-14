namespace Luna.CLI.Modules.Generator
{
	public class Module : BaseModule
	{
		public Module() : base("Generator", [
			new GenerateCommand()
		])
		{
		}
	}
}