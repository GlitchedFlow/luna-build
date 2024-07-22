// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Core module for the CLI.
	/// </summary>
	public class Module : BaseModule
	{
		internal Module() : base("Core", [
			new ListCommand(),
			new GetCommand(),
			new SetCommand(),
			new CompileCommand(),
			new InitCommand(),
			new LoadCommand()
		])
		{
			foreach (BaseModule module in RegisteredModules)
			{
				Commands.Add(new SwitchCommand(module.Name));
			}
		}

		/// <summary>
		/// Exits the CLI.
		/// </summary>
		public override void Exit()
		{
			ActiveModule = null;
		}
	}
}