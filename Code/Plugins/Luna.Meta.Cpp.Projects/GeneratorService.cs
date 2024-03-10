using Luna.Core;

namespace Luna.Meta.Cpp.Projects
{
	/// <summary>
	/// Meta service that can generate projects.
	/// </summary>
	public class GeneratorService : IMeta
	{
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService(this);
		}
	}
}