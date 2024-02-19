using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Core
{
	public class LunaConfig
	{
		public static LunaConfig Instance { get; private set; } = new();
		public string CorePath { get; set; } = typeof(IBuild).Assembly.Location;
		public string CodePath { get; set; } = "";
		public string MetaPath { get; set; } = "";
		public string SolutionPath { get; set; } = "";
		public List<string> Plugins { get; set; } = [];
		public List<string> Targets { get; set; } = [];
		public string Name { get; set; } = "";

		public static bool Load(string configPath)
		{
			LunaConfig? config = JsonSerializer.Deserialize(File.ReadAllText(configPath), typeof(LunaConfig), LunaConfigSourceGenerationContext.Default) as LunaConfig;
			if (config == null)
			{
				LunaConsole.ErrorLine($"Could not read config file.");
				return false;
			}

			string dirPath = Path.GetDirectoryName(configPath);
			if (!Path.IsPathFullyQualified(config.CorePath))
			{
				config.CorePath = Path.Combine(dirPath, config.CorePath);
			}
			if (!Path.IsPathFullyQualified(config.CodePath))
			{
				config.CodePath = Path.Combine(dirPath, config.CodePath);
			}
			if (!Path.IsPathFullyQualified(config.MetaPath))
			{
				config.MetaPath = Path.Combine(dirPath, config.MetaPath);
			}
			if (!Path.IsPathFullyQualified(config.SolutionPath))
			{
				config.SolutionPath = Path.Combine(dirPath, config.SolutionPath);
			}

			Instance = config;

			return true;
		}
	}

	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(LunaConfig))]
	internal partial class LunaConfigSourceGenerationContext : JsonSerializerContext
	{
	}
}