using Luna.Core;

namespace Luna.Meta.Cpp.Projects
{
	public class CCppProject : IMeta
	{
		public void Register()
		{
			CServiceProvider.RegistryService.RegisterMetaService(this);
		}
	}
}
