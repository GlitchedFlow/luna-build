using Luna.Core.Target;

namespace Luna.Core
{
    public interface IRegistryService
	{
		bool RegisterBuildService<T>(T buildService) where T : IBuild;
		T? GetBuildService<T>() where T : IBuild;
		IBuild? GetBuildService(Guid guid);
		int GetBuildServiceCount();
		IBuild? GetBuildServiceAt(int index);

		bool RegisterMetaService<T>(T metaService) where T : IMeta;
		T? GetMetaService<T>() where T : IMeta;
		IMeta? GetMetaService(Guid guid);

		bool RegisterTarget<T>(T target) where T : ITarget;
		T? GetTarget<T>() where T : ITarget;
		ITarget? GetTarget(Guid guid);
	}

	public static class CServiceProvider
	{
		public static IRegistryService RegistryService { get => Core.RegistryService.Instance; }
	}
}