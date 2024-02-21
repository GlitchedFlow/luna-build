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
		/// Generate the soltion for the active target.
		/// </summary>
		/// <returns>True if success, otherwise false.</returns>
		public bool Generate()
		{
			if (ActiveTarget == null)
			{
				return false;
			}

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