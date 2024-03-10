using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Core
{
	/// <summary>
	/// Config class that provides all details from the config file.
	/// </summary>
	public class LunaConfig
	{
		/// <summary>
		/// Gets the singleton instance for the config. If null then config has not yet been loaded.
		/// </summary>
		public static LunaConfig? Instance { get; private set; }

		/// <summary>
		/// Gets or sets the path to the core dll for the luna bridge.
		/// </summary>
		public string CorePath { get; set; } = $"{AppDomain.CurrentDomain.BaseDirectory}Core.dll";

		/// <summary>
		/// Gets or sets the path to where the source code (and with that all *.build.cs files) are located.
		/// </summary>
		public string CodePath { get; set; } = "";

		/// <summary>
		/// Gets or sets the path to where meta services are located.
		/// </summary>
		public string MetaPath { get; set; } = "";

		/// <summary>
		/// Gets or sets the path to where the solution should be generated.
		/// </summary>
		public string SolutionPath { get; set; } = "";

		/// <summary>
		/// Gets the workspace path.
		/// </summary>
		public string WorkspacePath { get; private set; } = "";

		/// <summary>
		/// Gets or sets the list of plugins which should be loaded.
		/// </summary>
		public List<string> Plugins { get; set; } = [];

		/// <summary>
		/// Gets or sets the list of targets which should be loaded.
		/// </summary>
		public List<string> Targets { get; set; } = [];

		/// <summary>
		/// Gets or sets the name of the solution.
		/// </summary>
		public string Name { get; set; } = "";

		/// <summary>
		/// Gets or sets if the luna bridge should be compiled in debug mode. Release mode if false.
		/// </summary>
		public bool CompileBridgeInDebug { get; set; } = false;

		/// <summary>
		/// Loads the config from the provided path.
		/// </summary>
		/// <param name="configPath">Path to the config file.</param>
		/// <returns>True if success, otherwise false.</returns>
		public static bool Load(string configPath)
		{
			if (Instance != null)
			{
				return false;
			}

			if (!File.Exists(configPath))
			{
				Log.Error($"Config file doesn't exist. Path: {configPath}");
				return false;
			}

			LunaConfig? config = JsonSerializer.Deserialize(File.ReadAllText(configPath), typeof(LunaConfig), LunaConfigSourceGenerationContext.Default) as LunaConfig;
			if (config == null)
			{
				Log.Error($"Could not read config file.");
				return false;
			}

			string? dirPath = Path.GetDirectoryName(configPath);
			if (dirPath == null)
			{
				Log.Error($"{configPath} is not valid directory path.");
				return false;
			}

			if (!Path.IsPathFullyQualified(config.CorePath))
			{
				config.CorePath = Path.GetFullPath(Path.Combine(dirPath, config.CorePath));
			}
			if (!Path.IsPathFullyQualified(config.CodePath))
			{
				config.CodePath = Path.GetFullPath(Path.Combine(dirPath, config.CodePath));
			}
			if (!Path.IsPathFullyQualified(config.MetaPath))
			{
				config.MetaPath = Path.GetFullPath(Path.Combine(dirPath, config.MetaPath));
			}
			if (!Path.IsPathFullyQualified(config.SolutionPath))
			{
				config.SolutionPath = Path.GetFullPath(Path.Combine(dirPath, config.SolutionPath));
			}
			if (!Path.IsPathFullyQualified(config.WorkspacePath))
			{
				config.WorkspacePath = Path.GetFullPath(Path.Combine(dirPath, config.WorkspacePath));
				if (!Path.Exists(config.WorkspacePath))
				{
					config.WorkspacePath = dirPath;
				}
			}

			Instance = config;

			if (!Directory.Exists(Cache.GetCacheFolder()))
			{
				Directory.CreateDirectory(Cache.GetCacheFolder());
			}

			return true;
		}
	}

	/// <summary>
	/// Context generator class for Luna Config.
	/// </summary>
	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(LunaConfig))]
	internal partial class LunaConfigSourceGenerationContext : JsonSerializerContext
	{
	}
}