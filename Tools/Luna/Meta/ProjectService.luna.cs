using Luna.Core;
using Luna.Targets.VisualStudio;
using System.Runtime.CompilerServices;

namespace Luna.BuildScript.Meta
{
	/// <summary>
	/// Project service to attach project specific information.
	/// </summary>
	public class ProjectService : IMeta
	{
		private ProjectEntry? _activeProjectTree = null;
		private string _outputPath = "";

		public const string AvaloniaVersion = "11.0.0";
		public const string ProjectExtension = "csproj";

		public ProjectService()
		{
			_outputPath = Path.Combine(LunaConfig.Instance.CodePath, "..\\bin");
		}

		/// <summary>
		/// Scope to handle the active project tree for the service.
		/// </summary>
		public class Scope : IDisposable
		{
			private ProjectEntry? _previousProjectTree = null;

			public Scope(ProjectEntry root)
			{
				ProjectService? service = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
				if (service != null)
				{
					_previousProjectTree = service._activeProjectTree;
					service._activeProjectTree = root;
				}
			}

			void IDisposable.Dispose()
			{
				ProjectService? service = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
				if (service != null)
				{
					service._activeProjectTree = _previousProjectTree;
				}
			}
		}

		/// <summary>
		/// Simple file record to map physical files to solution files. Optionally with a custom tag for the project file.
		/// </summary>
		/// <param name="physicalPath">Path to the file in the file system.</param>
		/// <param name="solutionPath">Relative path wher this file should be located in the solution.</param>
		/// <param name="elementTag">Optional tag for the project file. Defaults to "Compile"</param>
		public record FileRecord(string physicalPath, string solutionPath, string elementTag = "Compile");

		/// <summary>
		/// Registers the service.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService(this);

			// Set solution name.
			LunaConfig.Instance.Name = "Luna";
		}

		/// <summary>
		/// Sets the base information on the project file.
		/// </summary>
		/// <returns>Project Service</returns>
		private ProjectService BaseSetup()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			_activeProjectTree?.Attributes.Add("Sdk", "Microsoft.NET.Sdk");

			return this;
		}

		/// <summary>
		/// Marks the project as a console launcher.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService ConsoleLauncher()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			ProjectEntry? propertyGroup = _activeProjectTree.Children.FirstOrDefault(x => x.Name == "PropertyGroup");
			propertyGroup?.Children.AddRange([
					new("OutputType", "Exe", [], []),
					new ("PublishAot", "true", [], []),
					new ("InvariantGlobalization", "true", [], [])
			]);

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).dll\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).pdb\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).exe\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).runtimeconfig.json\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y"
			]);

			return this;
		}

		/// <summary>
		/// Marks the project as an Avalonia launcher.
		/// </summary>
		/// <param name="path">Path to where to physical files for the assets can be located. Will attach "Assets\\**" to the given path. Defaults to the file that calls.</param>
		/// <returns>Project Service</returns>
		public ProjectService AvalonLauncher([CallerFilePath] string? path = null)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			ProjectEntry? propertyGroup = _activeProjectTree.Children.FirstOrDefault(x => x.Name == "PropertyGroup");
			propertyGroup?.Children.AddRange([
					new("LangVersion", "latest", [], []),
					new ("AvaloniaUseCompiledBindingsByDefault", "true", [], [])
			]);

			_activeProjectTree.Children.Add(new("ItemGroup", "", [
				new("AvaloniaResource", "", [], new()
				{
					{ "Include", Path.Combine(Path.GetDirectoryName(path) ?? "", "Assets\\**") },
					{ "Link", "Assets\\%(RecursiveDir)%(Filename)%(Extension)" }
				})
			], []));

			_activeProjectTree.Children.Add(new("ItemGroup", "", [
				new("PackageReference", "", [], new()
				{
					{ "Include", "Avalonia"},
					{ "Version", AvaloniaVersion}
				}),
				new("PackageReference", "", [], new()
				{
					{ "Include", "Avalonia.Themes.Fluent"},
					{ "Version", AvaloniaVersion}
				}),
				new("PackageReference", "", [], new()
				{
					{ "Include", "Avalonia.Fonts.Inter"},
					{ "Version", AvaloniaVersion}
				}),
				new("PackageReference", "", [], new()
				{
					{ "Include", "CommunityToolkit.Mvvm"},
					{ "Version", "8.2.0"}
				}),
				new("PackageReference", "", [], new()
				{
					{ "Condition", "'$(Configuration)' == 'Debug'"},
					{ "Include", "Avalonia.Diagnostics" },
					{ "Version", AvaloniaVersion}
				}),
			], []));

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)\\\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y /s"
			]);

			return this;
		}

		/// <summary>
		/// Marks the project as Avalonia desktop launcher.
		/// </summary>
		/// <param name="pathToManiFest">Relative or absolute path to the app manifest.</param>
		/// <param name="path">Path that is used as base for relative path resolve for the app manifest. Defaults to the file location that calls.</param>
		/// <returns>Project Service</returns>
		public ProjectService AvalonDesktopLauncher(string pathToManiFest, [CallerFilePath] string? path = null)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			string manifestPath = Path.IsPathFullyQualified(pathToManiFest) ? pathToManiFest : Path.Combine(Path.GetDirectoryName(path) ?? "", pathToManiFest);
			if (!File.Exists(manifestPath))
			{
				ServiceProvider.LogService?.LogError($"File: {manifestPath} does not exist.");
			}

			Library();

			ProjectEntry? propertyGroup = _activeProjectTree.Children.FirstOrDefault(x => x.Name == "PropertyGroup");
			propertyGroup?.Children.AddRange([
					new ("OutputType", "WinExe", [], []),
					new ("PublishAot", "true", [], []),
					new ("InvariantGlobalization", "true", [], []),
					new ("BuiltInComInteropSupport", "true", [], []),
					new ("ApplicationManifest", manifestPath, [], [])
			]);

			_activeProjectTree.Children.Add(new("ItemGroup", "", [
				new("PackageReference", "", [], new()
				{
					{ "Include", "Avalonia.Desktop"},
					{ "Version", AvaloniaVersion},
				})
			], []));

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)\\\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y /s"
			]);

			return this;
		}

		/// <summary>
		/// Marks the project as a library.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService Library()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			BaseSetup();

			_activeProjectTree.Children.Add(new("PropertyGroup", "",
			[
				new ("TargetFramework", "net8.0", [], []),
				new ("ImplicitUsings", "enable", [], []),
				new ("Nullable", "enable", [], []),
			], []));

			return this;
		}

		/// <summary>
		/// Marks the project as a standalone library.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService StandaloneLibrary()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).dll\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).pdb\" \"{_outputPath}\\$(Platform)_$(Configuration)\\\" /y"
			]);

			return this;
		}

		/// <summary>
		/// File records which should be added to the project.
		/// </summary>
		/// <param name="files">Records of files that should be added to the project.</param>
		/// <param name="codeLocation">Location that is used to resolve relative paths from the records. Defaults to the file that calls.</param>
		/// <returns>Project Service</returns>
		public ProjectService Files(List<FileRecord> files, [CallerFilePath] string? codeLocation = null)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			ProjectEntry itemGroup = new("ItemGroup", "", [], []);

			foreach (FileRecord file in files)
			{
				string path = Path.IsPathFullyQualified(file.physicalPath) ? file.physicalPath : Path.Combine(Path.GetDirectoryName(codeLocation) ?? "", file.physicalPath);
				if (!File.Exists(path))
				{
					ServiceProvider.LogService?.LogError($"File: {path} does not exist.");
					continue;
				}

				itemGroup.Children.Add(new(file.elementTag, "", [], new()
				{
					{ "Include", $"{path}"},
					{ "Link", $"{file.solutionPath}"}
				}));
			}

			itemGroup.Children.Add(new("None", "", [], new()
			{
				{ "Include", $"{codeLocation}"},
				{ "Link", $"{Path.GetFileName(codeLocation)}" }
			}));

			_activeProjectTree.Children.Add(itemGroup);

			return this;
		}

		/// <summary>
		/// References which the project should have.
		/// </summary>
		/// <param name="projects">List of project names which should be referenced/</param>
		/// <returns>Project Service</returns>
		public ProjectService Reference(List<string> projects)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			ProjectEntry itemGroup = new("ItemGroup", "", [], []);

			foreach (string reference in projects)
			{
				itemGroup.Children.Add(new("ProjectReference", "", [], new()
				{
					{ "Include", $"{LunaConfig.Instance.SolutionPath}\\**\\{reference}.csproj" }
				}));
			}

			_activeProjectTree.Children.Add(itemGroup);

			return this;
		}

		/// <summary>
		/// Adds post build commands for the project.
		/// </summary>
		/// <param name="commands">List of commands</param>
		/// <returns>Project Service</returns>
		public ProjectService PostBuildCommands(List<string> commands)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			ProjectEntry target = new ProjectEntry("Target", "", [], new()
			{
				{ "Name", "PostBuild" },
				{ "AfterTargets", "PostBuildEvent" }
			});

			foreach (string command in commands)
			{
				target.Children.Add(new("Exec", "", [], new()
				{
					{ "Command", command.Replace("\"", "&quot;") }
				}));
			}

			_activeProjectTree.Children.Add(target);

			return this;
		}

		/// <summary>
		/// Adds pre build commands for the project.
		/// </summary>
		/// <param name="commands">List of commands</param>
		/// <returns>Project Service</returns>
		public ProjectService PreBuildCommands(List<string> commands)
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			ProjectEntry target = new ProjectEntry("Target", "", [], new()
			{
				{ "Name", "PreBuild" },
				{ "BeforeTargets", "PreBuildEvent" }
			});

			foreach (string command in commands)
			{
				target.Children.Add(new("Exec", "", [], new()
				{
					{ "Command", command.Replace("\"", "&quot;") }
				}));
			}

			_activeProjectTree.Children.Add(target);

			return this;
		}

		/// <summary>
		/// Marks the project as a plugin.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService Plugin()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).dll\" \"{_outputPath}\\$(Platform)_$(Configuration)\\Plugins\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).pdb\" \"{_outputPath}\\$(Platform)_$(Configuration)\\Plugins\\\" /y"
			]);

			return this;
		}

		/// <summary>
		/// Marks the project as a target.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService Target()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).dll\" \"{_outputPath}\\$(Platform)_$(Configuration)\\Targets\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).pdb\" \"{_outputPath}\\$(Platform)_$(Configuration)\\Targets\\\" /y"
			]);

			return this;
		}

		/// <summary>
		/// Marks the project as a CLI plugin.
		/// </summary>
		/// <returns>Project Service</returns>
		public ProjectService CLIPlugin()
		{
			if (_activeProjectTree == null)
			{
				return this;
			}

			Library();

			PostBuildCommands([
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).dll\" \"{_outputPath}\\$(Platform)_$(Configuration)\\CLI\\Plugins\\\" /y",
				$"xcopy \"$(ProjectDir)$(OutDir)$(TargetName).pdb\" \"{_outputPath}\\$(Platform)_$(Configuration)\\CLI\\Plugins\\\" /y"
			]);

			return this;
		}
	}
}