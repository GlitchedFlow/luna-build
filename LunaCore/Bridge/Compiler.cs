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
			Log.Write("Compiling LunaBridge");

			Log.OpenScope();

			if (!File.Exists(configPath))
			{
				Log.Error($"Config file not found at {configPath}");
				return false;
			}

			string plugins = "";

			foreach (string requestedPlugin in LunaConfig.Instance.Plugins)
			{
				string fullPath = Path.Combine([Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins", requestedPlugin]);
				if (!File.Exists(fullPath))
				{
					Log.Error($"Plugin {fullPath} not found.");
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
					Log.Error($"Target {fullPath} not found.");
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
				Process compiler = new();
				compiler.StartInfo.UseShellExecute = false;
				compiler.StartInfo.RedirectStandardOutput = true;
				compiler.StartInfo.FileName = "dotnet";
				compiler.StartInfo.Arguments = $"build {projectPath}";
				compiler.Start();

				string compilerLog = compiler.StandardOutput.ReadToEnd();

				compiler.WaitForExit();

				foreach (var logLine in compilerLog.Split("\r\n"))
				{
					string trimmed = logLine.Trim();
					if (trimmed.Contains("info"))
					{
						Log.Info(trimmed);
					}
					else if (trimmed.Contains("warning"))
					{
						Log.Warning(trimmed);
					}
					else if (trimmed.Contains("error"))
					{
						Log.Error(trimmed);
					}
					else if (trimmed.Contains("succeed"))
					{
						Log.Succes(trimmed);
					}
					else
					{
						Log.Write(trimmed);
					}
				}

				if (compiler.ExitCode != 0)
				{
					Log.Error($"Compilation was not successful. Exit Code:{compiler.ExitCode}");

					Log.CloseScope();
					return false;
				}
			}
			catch (Exception)
			{
				Log.Error("Compiler not available. Please install .NET 8.0 SDK");

				Log.CloseScope();
				return false;
			}

			var assemblyConfigurationAttribute = typeof(Compiler).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

			string lunaBridgeCompiledPath = Path.Combine(lunaBridgePath, $"bin\\{buildConfigurationName}\\net8.0\\LunaBridge.dll");
			string lunaBridgeTargetPath = Path.Combine("", "LunaBridge.dll");

			File.Copy(lunaBridgeCompiledPath, lunaBridgeTargetPath, true);

			Log.CloseScope();
			return true;
		}
	}
}