// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using System.Reflection;

namespace Luna.CLI.Modules.Core
{
	/// <summary>
	/// Set command for the CLI.
	/// </summary>
	public class SetCommand : BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public override string Name => "set";

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public override string Description => "Sets the current value of the requested luna config setting";

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

			bool wasSet = false;
			string oldValue = $"{property.GetValue(LunaConfig.Instance)}";

			if (property.PropertyType == typeof(bool) && bool.TryParse(args[1], out bool boolSetting))
			{
				property.SetValue(LunaConfig.Instance, boolSetting);
				wasSet = true;
			}
			else if (property.PropertyType == typeof(int) && int.TryParse(args[1], out int intSetting))
			{
				property.SetValue(LunaConfig.Instance, intSetting);
				wasSet = true;
			}
			else if (property.PropertyType == typeof(string))
			{
				property.SetValue(LunaConfig.Instance, args[1]);
				wasSet = true;
			}
			else if (property.PropertyType == typeof(List<string>))
			{
				property.SetValue(LunaConfig.Instance, args[1..].ToList());
				wasSet = true;
			}

			if (wasSet)
			{
				logSerivce.Log($"Config::{property.Name} set to \"{property.GetValue(LunaConfig.Instance)}\" [was \"{oldValue}\"]");
				return true;
			}
			else
			{
				logSerivce.LogError($"Unsupported parameter value provided.");
				return false;
			}
		}
	}
}