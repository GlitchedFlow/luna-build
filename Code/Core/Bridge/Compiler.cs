using System.Diagnostics;
using System.Reflection;

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
		public static bool Compile()
		{
			Log.Write("Compiling LunaBridge");

			Log.OpenScope();

			string plugins = "";

			if (LunaConfig.Instance == null)
			{
				Log.Error($"Luna Config was not yet loaded.");
				return false;
			}

			string? applicationDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (applicationDir == null)
			{
				Log.Error("Application directory is unknown.");
				return false;
			}

			foreach (string requestedPlugin in LunaConfig.Instance.Plugins)
			{
				// Try workspace plugins.
				string fullPath = Path.Combine([LunaConfig.Instance.WorkspacePath, "Plugins", $"{requestedPlugin}.dll"]);
				if (!File.Exists(fullPath))
				{
					string workspacePath = fullPath;
					// Try application plugins.
					fullPath = Path.Combine([applicationDir, "Plugins", $"{requestedPlugin}.dll"]);
					if (!File.Exists(fullPath))
					{
						Log.Error($"Plugin {requestedPlugin} was neither located via \"{fullPath}\" nor \"{workspacePath}\". Skipping. Luna Bridge might not compile.");
						continue;
					}
				}

				plugins += $"\t\t<Reference Include=\"{Path.GetFileNameWithoutExtension(requestedPlugin)}\">\r\n" + $"\t\t\t<HintPath>{fullPath}</HintPath>\r\n" + "\t\t</Reference>\r\n";
			}

			string targets = "";

			foreach (string requestedTarget in LunaConfig.Instance.Targets)
			{
				// Try workspace plugins.
				string fullPath = Path.Combine([LunaConfig.Instance.WorkspacePath, "Targets", $"{requestedTarget}.dll"]);
				if (!File.Exists(fullPath))
				{
					string workspacePath = fullPath;
					// Try application plugins.
					fullPath = Path.Combine([applicationDir, "Targets", $"{requestedTarget}.dll"]);
					if (!File.Exists(fullPath))
					{
						Log.Error($"Target {requestedTarget} was neither located via \"{fullPath}\" nor \"{workspacePath}\". Skipping. Luna Bridge might not compile.");
						continue;
					}
				}

				targets += $"\t\t<Reference Include=\"{Path.GetFileNameWithoutExtension(requestedTarget)}\">\r\n" + $"\t\t\t<HintPath>{fullPath}</HintPath>\r\n" + "\t\t</Reference>\r\n";
			}

			string finalProject = _bridgeProject.Replace("%Luna.Core.dll%", LunaConfig.Instance.CorePath)
												.Replace("%Luna.Bridge.Build%", Path.Combine(LunaConfig.Instance.CodePath, "**\\*.build.cs"))
												.Replace("%Luna.Bridge.Meta%", Path.Combine(LunaConfig.Instance.MetaPath, "**\\*.meta.cs"))
												.Replace("%Luna.Bridge.Plugins%", plugins)
												.Replace("%Luna.Bridge.Targets%", targets);

			string lunaBridgePath = Path.Combine(LunaConfig.Instance.WorkspacePath, "Bridge");
			string projectPath = Path.Combine(lunaBridgePath, "Bridge.csproj");

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
				compiler.StartInfo.Arguments = $"build {projectPath} -c {(LunaConfig.Instance.CompileBridgeInDebug ? "Debug" : "Release")}";
				compiler.Start();

				string compilerLog = compiler.StandardOutput.ReadToEnd();

				compiler.WaitForExit();

				WriteCompilerLog(compilerLog);

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

			string lunaBridgeCompiledPath = Path.Combine(lunaBridgePath, $"bin\\{(LunaConfig.Instance.CompileBridgeInDebug ? "Debug" : "Release")}\\net8.0\\Bridge.dll");
			string lunaBridgeTargetPath = Path.Combine(LunaConfig.Instance.WorkspacePath, "Bridge.dll");

			File.Copy(lunaBridgeCompiledPath, lunaBridgeTargetPath, true);

			Log.CloseScope();
			return true;
		}

		/// <summary>
		/// Parses the log from the compiler.
		/// </summary>
		/// <param name="compilerLog">Output from the compiler.</param>
		private static void WriteCompilerLog(string compilerLog)
		{
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
		}
	}
}