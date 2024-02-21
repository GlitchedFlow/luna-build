using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Core service interface that is used to query and registration of services.
	/// </summary>
	public interface IRegistryService
	{
		/// <summary>
		/// Registers a new build service.
		/// </summary>
		/// <typeparam name="T">Type of the build service.</typeparam>
		/// <param name="buildService">The instance of the build service.</param>
		/// <returns>True if successful, otherwise false.</returns>
		bool RegisterBuildService<T>(T buildService) where T : IBuild;

		/// <summary>
		/// Gets a build service based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the build service.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		T? GetBuildService<T>() where T : IBuild;

		/// <summary>
		/// Gets a build service based on the given guid.
		/// </summary>
		/// <param name="guid">The guid of the build service.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		IBuild? GetBuildService(Guid guid);

		/// <summary>
		/// Gets the count of all registered build services.
		/// </summary>
		/// <returns>Count of build services.</returns>
		int GetBuildServiceCount();

		/// <summary>
		/// Gets the build service at the given index.
		/// </summary>
		/// <param name="index">Index of the build service.</param>
		/// <returns>Valid instance if in range, otherwise Null.</returns>
		IBuild? GetBuildServiceAt(int index);

		/// <summary>
		/// Registers a new meta service.
		/// </summary>
		/// <typeparam name="T">Type of the meta service.</typeparam>
		/// <param name="metaService">Instance of the meta service.</param>
		/// <returns>True if successful, otherwise false.</returns>
		bool RegisterMetaService<T>(T metaService) where T : IMeta;

		/// <summary>
		/// Gets a meta service based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the meta service.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		T? GetMetaService<T>() where T : IMeta;

		/// <summary>
		/// Gets a meta service based on the given guid.
		/// </summary>
		/// <param name="guid">Guid of the meta service.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		IMeta? GetMetaService(Guid guid);

		/// <summary>
		/// Registers a target.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <param name="target">Instance of the target.</param>
		/// <returns>True if successful, otherwise false.</returns>
		bool RegisterTarget<T>(T target) where T : ITarget;

		/// <summary>
		/// Gets a target based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		T? GetTarget<T>() where T : ITarget;

		/// <summary>
		/// Gets a target based on the given guid.
		/// </summary>
		/// <param name="guid">The guid of the target.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		ITarget? GetTarget(Guid guid);

		/// <summary>
		/// Gets the count of all available targets.
		/// </summary>
		/// <returns>Count of all targets.</returns>
		int GetTargetCount();

		/// <summary>
		/// Gets the target at the given index.
		/// </summary>
		/// <param name="index">Index of the target.</param>
		/// <returns>Valid target if index is in range, otherwise Null.</returns>
		ITarget? GetTargetAt(int index);
	}

	/// <summary>
	/// Helper class to provide access to the IRegistryService from anywhere.
	/// </summary>
	public static class ServiceProvider
	{
		public static IRegistryService RegistryService { get => Core.RegistryService.Instance; }
	}
}