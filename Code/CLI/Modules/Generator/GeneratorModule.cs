namespace Luna.CLI.Modules.Generator
{
	/// <summary>
	/// Generator module for the CLI.
	/// </summary>
	public class Module : BaseModule
	{
		public Module() : base("Generator", [
			new GenerateCommand()
		])
		{
		}
	}
}