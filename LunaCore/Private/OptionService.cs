using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Core
{
	internal class Option(Guid guid, string name) : Identifier(guid), IOption
	{
		public string Name { get; init; } = name;
		public string Description { get; set; } = "";
		public bool IsEnabled { get; set; }
		public string Category { get; set; } = "";
	}

	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(Dictionary<Guid, bool>))]
	internal partial class OptionDictSourceGenerationContext : JsonSerializerContext
	{
	}

	internal class OptionService : IOptionService
	{
		private const string c_optionsFile = "options.luna.cache";

		private Dictionary<Guid, Option> m_optionByGuid = [];

		public OptionService()
		{
		}

		public bool? IsOptionEnabled(Guid guid)
		{
			if (!m_optionByGuid.TryGetValue(guid, out Option? value))
			{
				return null;
			}

			return value.IsEnabled;
		}

		public bool? IsOptionEnabled(string name)
		{
			var matchingElements = m_optionByGuid.Where(x => x.Value.Name == name);
			if (matchingElements.Any())
			{
				return matchingElements.First().Value.IsEnabled;
			}

			return null;
		}

		public void Register()
		{
			// Register through interface to keep implementation private.
			RegistryService.Instance.RegisterMetaService((IOptionService)this);
		}

		public IOption? RegisterOption(Guid guid, string name, bool IsEnabled)
		{
			if (m_optionByGuid.TryGetValue(guid, out Option? value))
			{
				LunaConsole.ErrorLine($"An option with guid {guid} is already registered.");
				return null;
			}

			var matchingElements = m_optionByGuid.Where(x => x.Value.Name == name);
			if (matchingElements.Any())
			{
				LunaConsole.ErrorLine($"An option with name {name} is already registered.");
				return null;
			}

			Option option = new(guid, name)
			{
				IsEnabled = IsEnabled
			};

			m_optionByGuid[guid] = option;

			LunaConsole.WriteLine($"Added Option: {option.Name}: {(option.IsEnabled ? "ON" : "OFF")}");

			return option;
		}

		public bool RemoveOption(Guid guid)
		{
			return m_optionByGuid.Remove(guid);
		}



		internal bool SaveToFile()
		{
			string cacheFilePath = Path.Combine(Cache.GetCacheFolder(), c_optionsFile);

			Dictionary<Guid, bool> options = new();
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

		internal bool LoadFromFile()
		{
			string cacheFilePath = Path.Combine(Cache.GetCacheFolder(), c_optionsFile);
			if (!File.Exists(cacheFilePath))
			{
				return true;
			}

			try
			{
				Dictionary<Guid, bool>? options = JsonSerializer.Deserialize(File.ReadAllText(cacheFilePath), typeof(Dictionary<Guid, bool>), OptionDictSourceGenerationContext.Default) as Dictionary<Guid, bool>;
				if (options == null)
				{
					LunaConsole.ErrorLine($"Could not read options cache.");
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
				LunaConsole.ErrorLine($"Could not read options cache.");
				return false;
			}

			return true;
		}

		internal void Clear()
		{
			m_optionByGuid.Clear();
		}
	}
}
