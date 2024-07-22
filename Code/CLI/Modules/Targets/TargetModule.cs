// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

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