using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Core service class that is used to generate the solution.
	/// </summary>
	internal class GeneratorService : IGeneratorService
	{
		/// <summary>
		/// Gets or sets the active target for the service.
		/// </summary>
		public ITarget? ActiveTarget { get; set; } = null;

		/// <summary>
		/// Generate the solution for the active target.
		/// </summary>
		/// <returns>True if success, otherwise false.</returns>
		public bool Generate()
		{
			// Save current option setup.
			((OptionService?)ServiceProvider.OptionService)?.SaveToFile();

			if (ActiveTarget == null)
			{
				return false;
			}

			ServiceProvider.RegistryService.GetSourceCodeLocation(this);

			return ActiveTarget.GenerateSolution();
		}

		/// <summary>
		/// Registers the service. Called by system.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService((IGeneratorService)this);
		}
	}
}