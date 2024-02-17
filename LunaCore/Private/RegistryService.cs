using Luna.Core.Target;

namespace Luna.Core
{
    internal class RegistryService : IRegistryService
	{
		#region Members
		private static RegistryService _instance = new();
		private Dictionary<Guid, IBuild> _buildServiceByGuid = new();
		private Dictionary<Guid, IMeta> _metaServiceByGuid = new();
		private Dictionary<Guid, ITarget> _targetByGuid = new();
		#endregion

		#region Properties
		internal static RegistryService Instance { get => _instance; }
		#endregion

		#region Constructor
		private RegistryService()
		{

		}
		#endregion

		public bool RegisterBuildService<T>(T buildService) where T : IBuild
		{
			if (buildService != null)
			{
				System.Type type = typeof(T);

				if (GetBuildService(type.GUID) != null)
				{
					LunaConsole.WarningLine($"Build service {type.FullName} was already registered.");
					return false;
				}

				_buildServiceByGuid.Add(type.GUID, buildService);
				LunaConsole.InfoLine($"Added Build Service: {type.FullName} with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		public T? GetBuildService<T>() where T : IBuild
		{
			System.Type buildType = typeof(T);
			return (T?)GetBuildService(buildType.GUID);
		}

		public IBuild? GetBuildService(Guid guid)
		{
			if (_buildServiceByGuid.ContainsKey(guid))
			{
				return _buildServiceByGuid[guid];
			}

			return null;
		}

		public int GetBuildServiceCount()
		{
			return _buildServiceByGuid.Count;
		}

		public IBuild? GetBuildServiceAt(int index)
		{
			if (index >= GetBuildServiceCount())
			{
				LunaConsole.ErrorLine($"{index} is out of range for registered build services.");
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

		public bool RegisterMetaService<T>(T metaService) where T : IMeta
		{
			if (metaService != null)
			{
				System.Type type = typeof(T);

				if (GetMetaService(type.GUID) != null)
				{
					LunaConsole.WarningLine($"Meta service {type.FullName} was already registered.");
					return false;
				}

				_metaServiceByGuid.Add(type.GUID, metaService);
				LunaConsole.InfoLine($"Added Meta Service: {type.FullName} with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		public T? GetMetaService<T>() where T : IMeta
		{
			System.Type metaType = typeof(T);
			return (T?)GetMetaService(metaType.GUID);
		}

		public IMeta? GetMetaService(Guid guid)
		{
			if (!_metaServiceByGuid.ContainsKey(guid))
			{
				return null;
			}

			return _metaServiceByGuid[guid];
		}

		public bool RegisterTarget<T>(T target) where T : ITarget
		{
			if (target != null)
			{
				System.Type type = typeof(T);

				if (GetTarget(type.GUID) != null)
				{
					LunaConsole.WarningLine($"Target {type.FullName} was already registered.");
					return false;
				}

				_targetByGuid.Add(type.GUID, target);
				LunaConsole.InfoLine($"Added Target: {target.Name} [{type.FullName}] with GUID: {type.GUID}");

				return true;
			}

			return false;
		}

		public T? GetTarget<T>() where T : ITarget
		{
			System.Type targetType = typeof(T);
			return (T?)GetTarget(targetType.GUID);
		}

		public ITarget? GetTarget(Guid guid)
		{
			if (!_targetByGuid.ContainsKey(guid))
			{
				return null;
			}

			return _targetByGuid[guid];
		}
	}
}