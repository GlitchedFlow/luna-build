using Luna.Core;

namespace Luna.Meta.Cpp.Projects
{
	public class CCppProject : IMeta
	{
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService(this);
		}
	}
}