using Luna.Core.Target;

namespace Luna.Core
{
	internal class RegistryService : IRegistryService
	{
		#region Members

		private static RegistryService _instance = new();
		private Dictionary<Guid, IBuild> _buildServiceByGuid = [];
		private Dictionary<Guid, IMeta> _metaServiceByGuid = [];
		private Dictionary<Guid, ITarget> _targetByGuid = [];

		#endregion Members

		#region Properties

		/// <summary>
		/// Gets the singleton instances of the registry service.
		/// </summary>
		internal static RegistryService Instance => _instance;

		#endregion Properties

		#region Constructor

		private RegistryService()
		{
		}

		#endregion Constructor

		/// <summary>
		/// Registers a new build service.
		/// </summary>
		/// <typeparam name="T">Type of the build service.</typeparam>
		/// <param name="buildService">The instance of the build service.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public bool RegisterBuildService<T>(T buildService) where T : IBuild
		{
			if (buildService != null)
			{
				System.Type type = typeof(T);

				if (GetBuildService(type.GUID) != null)
				{
					Log.Warning($"Build service {type.FullName} was already registered.");
					return false;
				}

				_buildServiceByGuid.Add(type.GUID, buildService);
				Log.Info($"Added Build Service: {type.FullName} with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets a build service based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the build service.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public T? GetBuildService<T>() where T : IBuild
		{
			System.Type buildType = typeof(T);
			return (T?)GetBuildService(buildType.GUID);
		}

		/// <summary>
		/// Gets a build service based on the given guid.
		/// </summary>
		/// <param name="guid">The guid of the build service.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public IBuild? GetBuildService(Guid guid)
		{
			if (_buildServiceByGuid.ContainsKey(guid))
			{
				return _buildServiceByGuid[guid];
			}

			return null;
		}

		/// <summary>
		/// Gets the count of all registered build services.
		/// </summary>
		/// <returns>Count of build services.</returns>
		public int GetBuildServiceCount()
		{
			return _buildServiceByGuid.Count;
		}

		/// <summary>
		/// Gets the build service at the given index.
		/// </summary>
		/// <param name="index">Index of the build service.</param>
		/// <returns>Valid instance if in range, otherwise Null.</returns>
		public IBuild? GetBuildServiceAt(int index)
		{
			if (index >= GetBuildServiceCount())
			{
				Log.Error($"{index} is out of range for registered build services.");
				return null;
			}

			int curIndex = 0;
			foreach (var item in _buildServiceByGuid)
			{
				if (curIndex == index)
				{
					return item.Value;
				}
				++curIndex;
			}

			return null;
		}

		/// <summary>
		/// Registers a new meta service.
		/// </summary>
		/// <typeparam name="T">Type of the meta service.</typeparam>
		/// <param name="metaService">Instance of the meta service.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public bool RegisterMetaService<T>(T metaService) where T : IMeta
		{
			if (metaService != null)
			{
				System.Type type = typeof(T);

				if (GetMetaService(type.GUID) != null)
				{
					Log.Warning($"Meta service {type.FullName} was already registered.");
					return false;
				}

				_metaServiceByGuid.Add(type.GUID, metaService);
				Log.Info($"Added Meta Service: {type.FullName} with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets a meta service based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the meta service.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public T? GetMetaService<T>() where T : IMeta
		{
			System.Type metaType = typeof(T);
			return (T?)GetMetaService(metaType.GUID);
		}

		/// <summary>
		/// Gets a meta service based on the given guid.
		/// </summary>
		/// <param name="guid">Guid of the meta service.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public IMeta? GetMetaService(Guid guid)
		{
			if (!_metaServiceByGuid.ContainsKey(guid))
			{
				return null;
			}

			return _metaServiceByGuid[guid];
		}

		/// <summary>
		/// Registers a target.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <param name="target">Instance of the target.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public bool RegisterTarget<T>(T target) where T : ITarget
		{
			if (target != null)
			{
				System.Type type = typeof(T);

				if (GetTarget(type.GUID) != null)
				{
					Log.Warning($"Target {type.FullName} was already registered.");
					return false;
				}

				_targetByGuid.Add(type.GUID, target);
				Log.Info($"Added Target: {target.Name} [{type.FullName}] with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets a target based on the given type.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public T? GetTarget<T>() where T : ITarget
		{
			System.Type targetType = typeof(T);
			return (T?)GetTarget(targetType.GUID);
		}

		/// <summary>
		/// Gets a target based on the given guid.
		/// </summary>
		/// <param name="guid">The guid of the target.</param>
		/// <returns>Valid instance if available, otherwise Null.</returns>
		public ITarget? GetTarget(Guid guid)
		{
			if (!_targetByGuid.ContainsKey(guid))
			{
				return null;
			}

			return _targetByGuid[guid];
		}

		/// <summary>
		/// Gets the count of all available targets.
		/// </summary>
		/// <returns>Count of all targets.</returns>
		public int GetTargetCount()
		{
			return _targetByGuid.Count;
		}

		/// <summary>
		/// Gets the target at the given index.
		/// </summary>
		/// <param name="index">Index of the target.</param>
		/// <returns>Valid target if index is in range, otherwise Null.</returns>
		public ITarget? GetTargetAt(int index)
		{
			if (index >= GetTargetCount())
			{
				Log.Error($"{index} is out of range for registered targets.");
				return null;
			}

			int curIndex = 0;
			foreach (var item in _targetByGuid)
			{
				if (curIndex == index)
				{
					return item.Value;
				}
				++curIndex;
			}

			return null;
		}
	}
}