using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Core
{
	/// <summary>
	/// Internal class to represent an option.
	/// </summary>
	/// <param name="guid">Guid of the option.</param>
	/// <param name="name">Name of the option.</param>
	internal class Option(Guid guid, string name) : Identifier(guid), IOption
	{
		/// <summary>
		/// Gets the name of the option.
		/// </summary>
		public string Name { get; init; } = name;

		/// <summary>
		/// Gets or sets the description of the option.
		/// </summary>
		public string Description { get; set; } = "";

		/// <summary>
		/// Gets or sets if the option is enabled.
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the category of the option.
		/// </summary>
		public string Category { get; set; } = "";

		/// <summary>
		/// Gets or sets the option guid which this option depends on.
		/// </summary>
		public Guid DependsOn { get; set; } = Guid.Empty;
	}

	/// <summary>
	/// Serializer Context to add serialize support for options.
	/// </summary>
	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(Dictionary<Guid, bool>))]
	internal partial class OptionDictSourceGenerationContext : JsonSerializerContext
	{
	}

	/// <summary>
	/// Core service class that is used to handle options for the generation.
	/// </summary>
	internal class OptionService : IOptionService
	{
		private const string c_optionsFile = "options.luna.cache";

		private Dictionary<Guid, Option> m_optionByGuid = [];

		public OptionService()
		{
		}

		/// <summary>
		/// Checks if an option is enabled.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <returns>Returns true if enabled, otherwise false. Null if option does not exist.</returns>
		public bool? IsOptionEnabled(Guid guid)
		{
			if (!m_optionByGuid.TryGetValue(guid, out Option? value))
			{
				return null;
			}

			return value.IsEnabled;
		}

		/// <summary>
		/// Checks if an option is enabled.
		/// </summary>
		/// <param name="name">The name of the option.</param>
		/// <returns>Returns true if enabled, otherwise false. Null if option does not exist.</returns>
		public bool? IsOptionEnabled(string name)
		{
			var matchingElements = m_optionByGuid.Where(x => x.Value.Name == name);
			if (matchingElements.Any())
			{
				return matchingElements.First().Value.IsEnabled;
			}

			return null;
		}

		/// <summary>
		/// Registers the service. Called by system.
		/// </summary>
		public void Register()
		{
			// Register through interface to keep implementation private.
			RegistryService.Instance.RegisterMetaService((IOptionService)this);
		}

		/// <summary>
		/// Registers a new option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="IsEnabled">Is this option enabled?</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		public IOption? RegisterOption(Guid guid, string name, bool IsEnabled)
		{
			if (m_optionByGuid.TryGetValue(guid, out Option? value))
			{
				Log.Error($"An option with guid {guid} is already registered.");
				return null;
			}

			var matchingElements = m_optionByGuid.Where(x => x.Value.Name == name);
			if (matchingElements.Any())
			{
				Log.Error($"An option with name {name} is already registered.");
				return null;
			}

			Option option = new(guid, name)
			{
				IsEnabled = IsEnabled
			};

			m_optionByGuid[guid] = option;

			Log.Write($"Added Option: {option.Name}: {(option.IsEnabled ? "ON" : "OFF")}");

			return option;
		}

		/// <summary>
		/// Removes an option based on the guid.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <returns>True if removed, otherwise false.</returns>
		public bool RemoveOption(Guid guid)
		{
			return m_optionByGuid.Remove(guid);
		}

		/// <summary>
		/// Saves the current state of options to file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		public bool SaveToFile()
		{
			string cacheFilePath = Path.Combine(Cache.GetCacheFolder(), c_optionsFile);

			Dictionary<Guid, bool> options = [];
			foreach (var opt in m_optionByGuid)
			{
				options.Add(opt.Key, opt.Value.IsEnabled);
			}

			string optionsJSON = JsonSerializer.Serialize(options, typeof(Dictionary<Guid, bool>), OptionDictSourceGenerationContext.Default);

			if (!Directory.Exists(Cache.GetCacheFolder()))
			{
				Directory.CreateDirectory(Cache.GetCacheFolder());
			}

			File.WriteAllText(cacheFilePath, optionsJSON);

			return true;
		}

		/// <summary>
		/// Loads options state from file and tries to apply the state of each if available.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		public bool LoadFromFile()
		{
			string cacheFilePath = Path.Combine(Cache.GetCacheFolder(), c_optionsFile);
			if (!File.Exists(cacheFilePath))
			{
				return true;
			}

			try
			{
				if (JsonSerializer.Deserialize(File.ReadAllText(cacheFilePath), typeof(Dictionary<Guid, bool>), OptionDictSourceGenerationContext.Default) is not Dictionary<Guid, bool> options)
				{
					Log.Error($"Could not read options cache.");
					return false;
				}

				foreach (var loadedOption in options)
				{
					if (m_optionByGuid.TryGetValue(loadedOption.Key, out Option? value))
					{
						value.IsEnabled = loadedOption.Value;
					}
				}
			}
			catch (Exception)
			{
				Log.Error($"Could not read options cache.");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Clears all options.
		/// </summary>
		internal void Clear()
		{
			m_optionByGuid.Clear();
		}
	}
}