using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Core
{
	/// <summary>
	/// Internal class to represent an option.
	/// </summary>
	/// <param name="guid">Guid of the option.</param>
	/// <param name="name">Name of the option.</param>
	/// <param name="category">Name of the category.</param>
	/// <param name="dependsOn">Guid of the option this option depends on.</param>
	internal class Option(Guid guid, string name, string? category = null, Guid? dependsOn = null) : Identifier(guid), IOption
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
		/// Gets or sets the category of the option.
		/// </summary>
		public string Category { get; set; } = category;

		/// <summary>
		/// Gets or sets the option guid which this option depends on.
		/// </summary>
		public Guid DependsOn { get; set; } = dependsOn ?? Guid.Empty;

		/// <summary>
		/// Gets the list of options which depend on this option.
		/// </summary>
		internal List<Option> DependencyOf { get; } = [];
	}

	/// <summary>
	/// Internal class to represent a flag option.
	/// </summary>
	/// <param name="guid">Guid of the option.</param>
	/// <param name="name">Name of the option.</param>
	/// <param name="isEnabled">Is this option enabled by default.</param>
	/// <param name="category">Category of the option.</param>
	/// <param name="dependsOn">Guid of the option this option depends on.</param>
	internal class FlagOption(Guid guid, string name, bool isEnabled, string? category = null, Guid? dependsOn = null) : Option(guid, name, category, dependsOn), IFlagOption
	{
		/// <summary>
		/// Gets or sets if the option is enabled.
		/// </summary>
		public bool IsEnabled { get; set; } = isEnabled;
	}

	/// <summary>
	/// Internal class to represent a value option.
	/// </summary>
	/// <param name="guid">Guid of the option.</param>
	/// <param name="name">Name of the option.</param>
	/// <param name="value">Value of the option.</param>
	/// <param name="category">Category of the option.</param>
	/// <param name="dependsOn">Guid of the option this option depends on.</param>
	internal class ValueOption(Guid guid, string name, string value, string? category = null, Guid? dependsOn = null) : Option(guid, name, category, dependsOn), IValueOption
	{
		/// <summary>
		/// Gets or sets if the option is enabled.
		/// </summary>
		public string Value { get; } = value;
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

			FlagOption? option = (FlagOption?)value;
			if (option == null)
			{
				// Handle anything else then a flag as true.
				return true;
			}

			return option.IsEnabled;
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
		/// Registers a new flag option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="IsEnabled">Is this option enabled?</param>
		/// <param name="category">Category of the option/</param>
		/// <param name="dependsOn">The guid of the option this option depends on.</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		public IFlagOption? RegisterFlagOption(Guid guid, string name, bool IsEnabled, string? category = null, Guid? dependsOn = null)
		{
			if (m_optionByGuid.TryGetValue(guid, out _))
			{
				Log.Error($"An option with guid {guid} is already registered.");
				return null;
			}

			FlagOption option = new(guid, name, IsEnabled, category, dependsOn);

			m_optionByGuid[guid] = option;

			Log.Write($"{option.Name}: {(option.IsEnabled ? "ON" : "OFF")}");

			return option;
		}

		/// <summary>
		/// Registers a new value option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="IsEnabled">Is this option enabled?</param>
		/// <param name="category">Category of the option/</param>
		/// <param name="dependsOn">The guid of the option this option depends on.</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		public IValueOption? RegisterValueOption(Guid guid, string name, string value, string? category = null, Guid? dependsOn = null)
		{
			if (m_optionByGuid.TryGetValue(guid, out _))
			{
				Log.Error($"An option with guid {guid} is already registered.");
				return null;
			}

			ValueOption option = new(guid, name, value, category, dependsOn);

			m_optionByGuid[guid] = option;

			Log.Write($"{option.Name}: {option.Value}");

			return option;
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
				FlagOption? option = (FlagOption?)opt.Value;
				if (option == null)
				{
					continue;
				}

				options.Add(opt.Key, option.IsEnabled);
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
						FlagOption? option = (FlagOption?)value;
						if (option == null)
						{
							continue;
						}

						option.IsEnabled = loadedOption.Value;
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

		public void BuildDependencyTree()
		{
			foreach (Option option in m_optionByGuid.Values)
			{
				if (option.DependsOn != Guid.Empty)
				{
					if (!m_optionByGuid.TryGetValue(option.DependsOn, out Option? value))
					{
						LogService.Instance.LogError($"No option with the guid: '{option.DependsOn}' was registered.");
						continue;
					}

					value.DependencyOf.Add(option);
				}
			}
		}

		public void VisitOptions(Func<IOption, bool> visitor)
		{
			foreach (Option option in m_optionByGuid.Values)
			{
				if (!visitor(option))
				{
					return;
				}
			}
		}

		public void VisitGroupedOptions(Func<IGrouping<string, IOption>, bool> visitor)
		{
			foreach (var group in m_optionByGuid.Values.GroupBy(x => x.Category))
			{
				if (!visitor(group))
				{
					return;
				}
			}
		}
	}
}