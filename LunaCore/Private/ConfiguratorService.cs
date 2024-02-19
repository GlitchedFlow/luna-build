namespace Luna.Core
{
	internal class ConfiguratorService : IConfiguratorService
	{
		public void Configurate()
		{
			OptionService? optionService = (OptionService?)RegistryService.Instance.GetMetaService<IOptionService>();
			if (optionService == null)
			{
				LunaConsole.ErrorLine($"Option Service was not registered.");
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

		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService((IConfiguratorService)this);
		}
	}
}