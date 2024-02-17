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
		/// Kickstart the Luna bridge.
		/// </summary>
		public void Initialize()
		{
			LunaConsole.WriteLine($"Initializing LunaBridge");

			LunaConsole.OpenScope();

			InitializeCoreServices();
			InitializeTargets();
			InitializePlugins();
			InitializeBridge();

			LunaConsole.CloseScope();
		}

		private void InitializePlugins()
		{
			LunaConsole.WriteLine($"Initializing Plugins");

			LunaConsole.OpenScope();

			DirectoryInfo pluginDir = new("./Plugins");

			if (!pluginDir.Exists)
			{
				LunaConsole.CloseScope();
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
					LunaConsole.ErrorLine($"Unable to handle {file.FullName}");
				}
			}

			LunaConsole.CloseScope();
		}

		private void InitializeBridge()
		{
			const string lunaBridgeDll = "LunaBridge.dll";

			if (!File.Exists(lunaBridgeDll))
			{
				LunaConsole.ErrorLine($"Luna Bridge assembly not found. Given path was: '${lunaBridgeDll}'");
				return;
			}

			LunaConsole.WriteLine($"Initializing Meta and Buildables ({lunaBridgeDll})");

			LunaConsole.OpenScope();

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
				LunaConsole.ErrorLine($"Luna Bridge initialization failed. Reason: {e}");

				LunaConsole.CloseScope();
				return;
			}

			OptionService? optionService = (OptionService?)RegistryService.Instance.GetMetaService<IOptionService>();
			if (optionService == null)
			{
				LunaConsole.ErrorLine($"Option Service was not registered.");
				return;
			}

			optionService.Clear();

			int serviceCount = CServiceProvider.RegistryService.GetBuildServiceCount();
			for (int curIndex = 0; curIndex < serviceCount; ++curIndex)
			{
				IBuild? build = CServiceProvider.RegistryService.GetBuildServiceAt(curIndex);
				build?.Configurate();
			}

			optionService.LoadFromFile();

			//if (!CRegistryService.Instance.GenerateSolution())
			//{
			//	CLunaConsole.ErrorLine("Generating solution failed.");
			//	return;
			//}

			optionService.SaveToFile();

			LunaConsole.CloseScope();
		}

		private void InitializeTargets()
		{
			LunaConsole.WriteLine($"Initializing Targets");

			LunaConsole.OpenScope();

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
						LunaConsole.ErrorLine($"Unable to handle {file.FullName}");
					}
				}
			}

			LunaConsole.CloseScope();
		}

		private void InitializeCoreServices()
		{
			LunaConsole.WriteLine($"Initializing Core Services");

			LunaConsole.OpenScope();

			try
			{
				Assembly coreAssembly = Assembly.GetAssembly(typeof(Kickstart));
				if (coreAssembly == null)
				{
					LunaConsole.ErrorLine("Core Services not found.");
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
				LunaConsole.ErrorLine($"Luna Bridge initialization failed. Reason: {e}");

				LunaConsole.CloseScope();
				return;
			}

			LunaConsole.CloseScope();
		}
	}
}