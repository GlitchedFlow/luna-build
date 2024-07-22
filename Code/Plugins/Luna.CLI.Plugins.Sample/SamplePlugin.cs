// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Plugins
{
	/// <summary>
	/// Sample command.
	/// </summary>
	internal class SampleCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "sample";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Sample command description.";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			ServiceProvider.LogService?.LogSuccess($"CLI plugin called.");
			return true;
		}
	}

	/// <summary>
	/// Custom module as plugin
	/// </summary>
	public class SamplePlugin : BaseModule
	{
		public SamplePlugin()
			: base("SamplePlugin", [
				new SampleCommand()
			])
		{
		}
	}
}