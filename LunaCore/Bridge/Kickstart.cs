using System.Reflection;
using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Class which kickstarts meta and build services within Luna Bridge.
	/// </summary>
	public class Kickstart
	{
		private const string _pluginsDir = "Plugins";
		private const string _targetsDir = "Targets";

		/// <summary>
		/// Scans and loads all types from a list of requested files in a directory.
		/// </summary>
		/// <typeparam name="TYPE">The type which should be loaded from matching dll files.</typeparam>
		/// <param name="dir">Directory info</param>
		/// <param name="requestedFiles">Requested files.</param>
		/// <param name="callback">Callback for the created instance.</param>
		private static void ScanDirAndLoadTypes<TYPE>(DirectoryInfo dir, List<string> requestedFiles, Action<TYPE> callback)
		{
			foreach (FileInfo file in dir.GetFiles("*.dll").Where(x => requestedFiles.Contains(Path.GetFileNameWithoutExtension(x.Name))))
			{
				requestedFiles.Remove(Path.GetFileNameWithoutExtension(file.FullName));

				try
				{
					Assembly pluginAssembly = Assembly.LoadFrom(file.FullName);

					foreach (Type type in pluginAssembly.GetTypes())
					{
						ConstructorInfo? defaultConstructor = type.GetConstructor([]);
						if (type.IsAssignableTo(typeof(TYPE)) && defaultConstructor != null && defaultConstructor.IsPublic)
						{
							TYPE instance = (TYPE)defaultConstructor.Invoke([]);
							callback(instance);
						}
					}
				}
				catch (Exception)
				{
					Log.Error($"Unable to handle {file.FullName}");
				}
			}
		}

		/// <summary>
		/// Initializes all plugins.
		/// </summary>
		public static void InitializePlugins()
		{
			Log.Write($"Initializing Plugins");

			Log.OpenScope();

			DirectoryInfo curPluginDir = new(Path.Combine(LunaConfig.Instance.WorkspacePath, _pluginsDir));

			List<string> requestedPlugins = LunaConfig.Instance.Plugins;

			if (curPluginDir.Exists)
			{
				ScanDirAndLoadTypes(curPluginDir, requestedPlugins, (IMeta instance) => instance.Register());
			}

			curPluginDir = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pluginsDir));
			if (curPluginDir.Exists)
			{
				ScanDirAndLoadTypes(curPluginDir, requestedPlugins, (IMeta instance) => instance.Register());
			}

			Log.CloseScope();
		}

		/// <summary>
		/// Initializes the luna bridge.
		/// </summary>
		public static void InitializeBridge()
		{
			Log.Write($"Initializing LunaBridge");

			string lunaBridgeDll = Path.Combine(LunaConfig.Instance.WorkspacePath, "LunaBridge.dll");

			if (!File.Exists(lunaBridgeDll))
			{
				Log.Error($"Luna Bridge assembly not found. Given path was: \"{lunaBridgeDll}\"");
				return;
			}

			Log.Write($"Initializing Meta and Buildables ({lunaBridgeDll})");

			Log.OpenScope();

			try
			{
				Assembly bridgeAssembly = Assembly.LoadFrom(lunaBridgeDll);

				List<ConstructorInfo> metaTypes = [];
				List<ConstructorInfo> buildTypes = [];

				foreach (Type type in bridgeAssembly.GetTypes())
				{
					ConstructorInfo? defaultConstructor = type.GetConstructor([]);
					if (type.IsAssignableTo(typeof(IMeta)) && defaultConstructor != null && defaultConstructor.IsPublic)
					{
						metaTypes.Add(defaultConstructor);
					}
					else if (type.IsAssignableTo(typeof(IBuild)) && defaultConstructor != null && defaultConstructor.IsPublic)
					{
						buildTypes.Add(defaultConstructor);
					}
				}

				foreach (ConstructorInfo defaultConstructor in metaTypes)
				{
					IMeta meta = (IMeta)defaultConstructor.Invoke([]);
					meta.Register();
				}

				foreach (ConstructorInfo defaultConstructor in buildTypes)
				{
					IBuild build = (IBuild)defaultConstructor.Invoke([]);
					build.Register();
				}
			}
			catch (Exception e)
			{
				Log.Error($"Luna Bridge initialization failed. Reason: {e}");

				Log.CloseScope();
				return;
			}

			Log.CloseScope();
		}

		/// <summary>
		/// Initializes all targets.
		/// </summary>
		public static void InitializeTargets()
		{
			Log.Write($"Initializing Targets");

			Log.OpenScope();

			DirectoryInfo curTargetsDir = new(Path.Combine(LunaConfig.Instance.WorkspacePath, _targetsDir));

			List<string> requestedTargets = LunaConfig.Instance.Targets;

			if (curTargetsDir.Exists)
			{
				ScanDirAndLoadTypes(curTargetsDir, requestedTargets, (ITarget instance) => instance.Register());
			}

			curTargetsDir = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _targetsDir));
			if (curTargetsDir.Exists)
			{
				ScanDirAndLoadTypes(curTargetsDir, requestedTargets, (ITarget instance) => instance.Register());
			}

			Log.CloseScope();
		}

		/// <summary>
		/// Initializes all core services.
		/// </summary>
		public static void InitializeCoreServices()
		{
			Log.Write($"Initializing Core Services");

			Log.OpenScope();

			try
			{
				Assembly? coreAssembly = Assembly.GetAssembly(typeof(Kickstart));
				if (coreAssembly == null)
				{
					Log.Error("Core Services not found.");

					Log.CloseScope();
					return;
				}

				foreach (Type type in coreAssembly.GetTypes())
				{
					ConstructorInfo? defaultConstructor = type.GetConstructor([]);
					if (type.IsAssignableTo(typeof(IMeta)) && defaultConstructor != null)
					{
						IMeta meta = (IMeta)defaultConstructor.Invoke([]);
						meta.Register();
					}
				}
			}
			catch (Exception e)
			{
				Log.Error($"Luna Bridge initialization failed. Reason: {e}");

				Log.CloseScope();
				return;
			}

			Log.CloseScope();
		}
	}
}