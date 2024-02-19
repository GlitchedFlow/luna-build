using Luna.Core.Target;

namespace Luna.Core
{
	internal class GeneratorService : IGeneratorService
	{
		public ITarget? ActiveTarget { get; set; } = null;

		public bool Generate()
		{
			if (ActiveTarget == null)
			{
				return false;
			}

			return ActiveTarget.GenerateSolution();
		}

		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService((IGeneratorService)this);
		}
	}
}