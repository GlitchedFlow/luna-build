using System.Reflection;
using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Class which kickstarts meta and build services within Luna Bridge.
	/// </summary>
	public class Kickstart
	{
		/// <summary>
		/// Initializes all plugins.
		/// </summary>
		public static void InitializePlugins()
		{
			Log.Write($"Initializing Plugins");

			Log.OpenScope();

			DirectoryInfo pluginDir = new("./Plugins");

			if (!pluginDir.Exists)
			{
				LogService.Instance.CloseScope();
				return;
			}

			foreach (FileInfo file in pluginDir.GetFiles("*.dll").Where(x => LunaConfig.Instance.Plugins.Contains(x.Name)))
			{
				try
				{
					Assembly pluginAssembly = Assembly.LoadFrom(file.FullName);

					foreach (Type type in pluginAssembly.GetTypes())
					{
						ConstructorInfo? defaultConstructor = type.GetConstructor([]);
						if (type.IsAssignableTo(typeof(IMeta)) && defaultConstructor != null && defaultConstructor.IsPublic)
						{
							IMeta metaService = (IMeta)defaultConstructor.Invoke([]);
							metaService.Register();
						}
					}
				}
				catch (Exception)
				{
					Log.Error($"Unable to handle {file.FullName}");
				}
			}

			Log.CloseScope();
		}

		/// <summary>
		/// Initializes the luna bridge.
		/// </summary>
		public static void InitializeBridge()
		{
			Log.Write($"Initializing LunaBridge");

			const string lunaBridgeDll = "LunaBridge.dll";

			if (!File.Exists(lunaBridgeDll))
			{
				Log.Error($"Luna Bridge assembly not found. Given path was: '${lunaBridgeDll}'");
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

			DirectoryInfo targetsDir = new("./Targets");

			if (targetsDir.Exists)
			{
				foreach (FileInfo file in targetsDir.GetFiles("*.dll").Where(x => LunaConfig.Instance.Targets.Contains(x.Name)))
				{
					try
					{
						Assembly targetAssembly = Assembly.LoadFrom(file.FullName);

						foreach (Type type in targetAssembly.GetTypes())
						{
							ConstructorInfo? defaultConstructor = type.GetConstructor([]);
							if (type.IsAssignableTo(typeof(ITarget)) && defaultConstructor != null && defaultConstructor.IsPublic)
							{
								ITarget target = (ITarget)defaultConstructor.Invoke([]);
								target.Register();
							}
						}
					}
					catch (Exception)
					{
						Log.Error($"Unable to handle {file.FullName}");
					}
				}
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
				Assembly coreAssembly = Assembly.GetAssembly(typeof(Kickstart));
				if (coreAssembly == null)
				{
					Log.Error("Core Services not found.");
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