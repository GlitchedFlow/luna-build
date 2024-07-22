// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using Luna.Core.Target;

namespace Luna.CLI.Modules.Targets
{
	/// <summary>
	/// Targets list command for the CLI.
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
		public override string Description => "List all requested targets";

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

			int targetCount = ServiceProvider.RegistryService.GetTargetCount();

			logSerivce.Log($"== Available Targets [Count: {targetCount}] == ");
			using LogScope scope = new();

			for (int curTargetIndex = 0; curTargetIndex < targetCount; ++curTargetIndex)
			{
				ITarget? target = ServiceProvider.RegistryService.GetTargetAt(curTargetIndex);
				if (target != null)
				{
					logSerivce.Log(target.Name);
				}
			}

			return true;
		}
	}
}