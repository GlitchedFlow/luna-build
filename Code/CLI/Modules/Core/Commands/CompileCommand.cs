// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Compile Command for the CLI.
	/// </summary>
	public class CompileCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "compile";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Compiles the luna bridge project";

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

			if (!Compiler.Compile())
			{
				logSerivce.LogError($"Compile step failed.");
				return false;
			}

			return true;
		}
	}
}