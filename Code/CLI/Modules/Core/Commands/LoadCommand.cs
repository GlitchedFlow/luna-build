using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Load command for the CLI.
	/// </summary>
	public class LoadCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "load";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Loads the luna config from the provided path.";

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

			if (args.Length == 0)
			{
				logSerivce.LogError($"Missing parameter.");
				return false;
			}

			string path = Path.IsPathFullyQualified(args[0]) ? args[0] : Path.Combine(Path.GetDirectoryName(Console.ActiveScript ?? "") ?? "", args[0]);

			if (!File.Exists(path))
			{
				logSerivce.LogError($"{path} is not valid file path");
				return false;
			}

			if (!LunaConfig.Load(path))
			{
				logSerivce.LogError($"Luna config was not loaded. Path provided: {path}");
				return false;
			}

			logSerivce.Log("Luna config was loaded.");

			if (!LunaConfig.Instance)
			{
				logSerivce.LogWarning($"Luna config is not valid");
			}

			return true;
		}
	}
}