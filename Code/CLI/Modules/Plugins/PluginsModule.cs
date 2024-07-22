// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

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