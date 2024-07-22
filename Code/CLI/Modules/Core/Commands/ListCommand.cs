// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// List command for the CLI.
	/// </summary>
	public class ListCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "list";

		/// <summary>
		/// Gets the description for the command.
		/// </summary>
		public override string Description => "List all modules";

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public override bool Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce != null)
			{
				logSerivce.Log($"== Registered Modules [Count: {BaseModule.RegisteredModules.Count}] == ");
				foreach (BaseModule module in BaseModule.RegisteredModules)
				{
					module.Help();
				}

				return true;
			}

			return false;
		}
	}
}