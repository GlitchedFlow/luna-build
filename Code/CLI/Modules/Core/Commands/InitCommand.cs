using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Init command for CLI.
	/// </summary>
	public class InitCommand : BaseCommand
	{
		private static List<string> _supportedArgs = ["targets", "plugins", "bridge"];

		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "init";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Initializes the given system for the luna bridge. Supports \"targets\", \"plugins\" and \"bridge\"";

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

			string requested = args[0].ToLower();

			if (requested == _supportedArgs[0])
			{
				Kickstart.InitializeTargets();
				return true;
			}
			else if (requested == _supportedArgs[1])
			{
				Kickstart.InitializePlugins();
				return true;
			}
			else if (requested == _supportedArgs[2])
			{
				Kickstart.InitializeBridge();
				return true;
			}

			logSerivce.LogError($"Unsupported argument: {requested}");

			return false;
		}
	}
}