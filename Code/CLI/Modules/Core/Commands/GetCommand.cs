using System.Reflection;
using Luna.Core;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Get command for the CLI.
	/// </summary>
	public class GetCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "get";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Gets the current value of the requested luna config setting. Available settings match the members of luna config in lower case.";

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

			string settingName = args[0].ToLower();

			Type lunaConfigType = typeof(LunaConfig);

			PropertyInfo? property = lunaConfigType.GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals(settingName, StringComparison.CurrentCultureIgnoreCase));
			if (property == null)
			{
				logSerivce.LogError($"Unknown setting requested. Requested: {settingName}");
				return false;
			}

			logSerivce.Log($"Config::{property.Name} = \"{property.GetValue(LunaConfig.Instance)}\"");

			return true;
		}
	}
}