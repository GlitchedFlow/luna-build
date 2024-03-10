namespace Luna.Core
{
	/// <summary>
	/// Core service class that is used to configurate build services.
	/// </summary>
	internal class ConfiguratorService : IConfiguratorService
	{
		/// <summary>
		/// Configurate all build services.
		/// </summary>
		public void Configurate()
		{
			OptionService? optionService = (OptionService?)ServiceProvider.OptionService;
			if (optionService == null)
			{
				Log.Error($"Option Service was not registered.");
				return;
			}

			optionService.Clear();

			int buildablesCount = ServiceProvider.RegistryService.GetBuildServiceCount();

			for (int curIndex = 0; curIndex < buildablesCount; ++curIndex)
			{
				IBuild? buildable = ServiceProvider.RegistryService.GetBuildServiceAt(curIndex);
				buildable?.Configurate();
			}

			optionService.LoadFromFile();

			optionService.SaveToFile();
		}

		/// <summary>
		/// Register the service. Called by system.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService((IConfiguratorService)this);
		}
	}
}