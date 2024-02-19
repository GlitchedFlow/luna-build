using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Luna.Core
{
	/// <summary>
	/// Compiler class that takes care of compiling Luna Bridge.
	/// </summary>
	public class Compiler
	{
		private static string _bridgeProject = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n\r\n"
												+ "\t<PropertyGroup>\r\n"
													+ "\t\t<TargetFramework>net8.0</TargetFramework>\r\n"
													+ "\t\t<ImplicitUsings>enable</ImplicitUsings>\r\n"
													+ "\t\t<Nullable>enable</Nullable>\r\n"
												+ "\t</PropertyGroup>\r\n\r\n"
												+ "\t<ItemGroup>\r\n"
													+ "\t\t<Reference Include=\"LunaCore\">\r\n"
														+ "\t\t\t<HintPath>%Luna.Core.dll%</HintPath>\r\n"
													+ "\t\t</Reference>\r\n"
													+ "%Luna.Bridge.Plugins%"
													+ "%Luna.Bridge.Targets%"
												+ "\t</ItemGroup>\r\n\r\n"
												+ "\t<ItemGroup>\r\n"
													+ "\t\t<Compile Include=\"%Luna.Bridge.Build%\" />\r\n"
													+ "\t\t<Compile Include=\"%Luna.Bridge.Meta%\" />\r\n"
												+ "\t</ItemGroup>\r\n"
											+ "</Project>\r\n";

		/// <summary>
		/// Compiles the Luna Bridge.
		/// </summary>
		/// <param name="configPath">Path to the compiler config</param>
		public static bool Compile(string configPath)
		{
			LunaConsole.WriteLine("Compiling LunaBridge");

			LunaConsole.OpenScope();

			if (!File.Exists(configPath))
			{
				LunaConsole.ErrorLine($"Config file not found at {configPath}");
				return false;
			}

			string plugins = "";

			foreach (string requestedPlugin in LunaConfig.Instance.Plugins)
			{
				string fullPath = Path.Combine([Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins", requestedPlugin]);
				if (!File.Exists(fullPath))
				{
					LunaConsole.ErrorLine($"Plugin {fullPath} not found.");
					continue;
				}

				plugins += $"\t\t<Reference Include=\"{Path.GetFileNameWithoutExtension(requestedPlugin)}\">\r\n" + $"\t\t\t<HintPath>{fullPath}</HintPath>\r\n" + "\t\t</Reference>\r\n";
			}

			string targets = "";

			foreach (string requestedTarget in LunaConfig.Instance.Targets)
			{
				string fullPath = Path.Combine([Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Targets", requestedTarget]);
				if (!File.Exists(fullPath))
				{
					LunaConsole.ErrorLine($"Target {fullPath} not found.");
					continue;
				}

				plugins += $"\t\t<Reference Include=\"{Path.GetFileNameWithoutExtension(requestedTarget)}\">\r\n" + $"\t\t\t<HintPath>{fullPath}</HintPath>\r\n" + "\t\t</Reference>\r\n";
			}

			string finalProject = _bridgeProject.Replace("%Luna.Core.dll%", LunaConfig.Instance.CorePath)
												.Replace("%Luna.Bridge.Build%", Path.Combine(LunaConfig.Instance.CodePath, "**\\*.build.cs"))
												.Replace("%Luna.Bridge.Meta%", Path.Combine(LunaConfig.Instance.MetaPath, "**\\*.meta.cs"))
												.Replace("%Luna.Bridge.Plugins%", plugins)
												.Replace("%Luna.Bridge.Targets%", targets);

			const string lunaBridgePath = "LunaBridge";
			string projectPath = Path.Combine(lunaBridgePath, "LunaBridge.csproj");

			if (!Directory.Exists(lunaBridgePath))
			{
				Directory.CreateDirectory(lunaBridgePath);
			}

			File.WriteAllText(projectPath, finalProject);

			try
			{
				Process compiler = Process.Start("dotnet", ["build", projectPath]);

				compiler.WaitForExit();

				if (compiler.ExitCode != 0)
				{
					LunaConsole.ErrorLine($"Compilation was not successful. Exit Code:{compiler.ExitCode}");

					LunaConsole.CloseScope();
					return false;
				}
			}
			catch (Exception)
			{
				LunaConsole.ErrorLine("Compiler not available. Please install .NET 8.0 SDK");

				LunaConsole.CloseScope();
				return false;
			}

			var assemblyConfigurationAttribute = typeof(Compiler).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

			string lunaBridgeCompiledPath = Path.Combine(lunaBridgePath, $"bin\\{buildConfigurationName}\\net8.0\\LunaBridge.dll");
			string lunaBridgeTargetPath = Path.Combine("", "LunaBridge.dll");

			File.Copy(lunaBridgeCompiledPath, lunaBridgeTargetPath, true);

			LunaConsole.CloseScope();
			return true;
		}
	}
}