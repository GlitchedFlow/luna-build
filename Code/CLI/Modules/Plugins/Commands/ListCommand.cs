// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Plugins
{
	/// <summary>
	/// Plugins list command for the CLI.
	/// </summary>
	public class ListCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "list";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "List all requested plugins";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce == null)
			{
				return false;
			}

			logSerivce.Log($"== Requested Plugins [Count: {LunaConfig.Instance.Plugins.Count}] == ");
			using LogScope scope = new();

			foreach (string plugin in LunaConfig.Instance.Plugins)
			{
				logSerivce.Log(plugin);
			}

			return true;
		}
	}
}