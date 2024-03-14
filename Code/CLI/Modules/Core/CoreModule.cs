namespace Luna.CLI.Modules.Core
{
	public class Module : BaseModule
	{
		internal Module() : base("Core", [
			new ListCommand()
		])
		{
			foreach (BaseModule module in RegisteredModules)
			{
				Commands.Add(new SwitchCommand(module.Name));
			}
		}

		public override void Exit()
		{
			ActiveModule = null;
		}
	}
}