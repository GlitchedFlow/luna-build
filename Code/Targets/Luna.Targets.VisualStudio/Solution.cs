// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using Luna.Core.Target;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luna.Targets.VisualStudio
{
	/// <summary>
	/// Record to track if a project file needs to be updated.
	/// </summary>
	/// <param name="ProjectGuid">Guid of the project</param>
	/// <param name="SourceCodeLocation">Build file location</param>
	/// <param name="ModifiedDate">Modified date of the build file</param>
	/// <param name="OptionsState">Options with which the project was generated</param>
	record ProjectCache(Guid ProjectGuid, string SourceCodeLocation, DateTime ModifiedDate, Dictionary<Guid, bool> OptionsState);

	/// <summary>
	/// Record to track if a solution file needs to be updated.
	/// </summary>
	/// <param name="SolutionGuid">Guid of the solution.</param>
	/// <param name="Projects">List of project files.</param>
	/// <param name="Platforms">List of platforms.</param>
	/// <param name="Profiles">List of profiles.</param>
	record SolutionCache(Guid SolutionGuid, List<string> Projects, List<string> Platforms, List<string> Profiles);

	/// <summary>
	/// Solution file generator for Visual Studio.
	/// </summary>
	/// <param name="solutionPath">Path to the solution file.</param>
	/// <param name="solutionGuid">Guid of the solution.</param>
	public class Solution(string solutionPath, Guid solutionGuid) : ISolution
	{
		private const string PROJECT_CACHE_FILE = "luna.project.cache";
		private const string SOLUTION_CACHE_FILE = "luna.solution.cache";

		/// <summary>
		/// Record to track the folder hierarchy in the solution.
		/// </summary>
		/// <param name="Name">Name of the folder.</param>
		/// <param name="Guid">Guid of the folder.</param>
		/// <param name="Parent">Parent folder.</param>
		record Folder(string Name, Guid Guid, Folder? Parent, bool NestedOnly);

		private const string _solutionTemplate =
			"\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n" +
			"# Visual Studio Version 17\r\nVisualStudioVersion = 17\r\n" +
			"MinimumVisualStudioVersion = 10\r\n" +
			"%Visual.Studio.Folders%" +
			"%Visual.Studio.Projects%" +
			"Global\r\n" +
			"\tGlobalSection(SolutionConfigurationPlatforms) = preSolution\r\n" +
			"%Visual.Studio.Platforms%" +
			"\tEndGlobalSection\r\n" +
			"\tGlobalSection(ProjectConfigurationPlatforms) = postSolution\r\n" +
			"%Visual.Studio.ProjectPlatforms%" +
			"\tEndGlobalSection\r\n" +
			"\tGlobalSection(SolutionProperties) = preSolution\r\n" +
			"\t\tHideSolutionNode = FALSE\r\n" +
			"\tEndGlobalSection\r\n" +
			"\tGlobalSection(NestedProjects) = preSolution\r\n" +
			"%Visual.Studio.NestedProjects%" +
			"\tEndGlobalSection\r\n" +
			"\tGlobalSection(ExtensibilityGlobals) = postSolution\r\n" +
			"\t\tSolutionGuid = %Visual.Studio.SolutionGuid%\r\n" +
			"\tEndGlobalSection\r\n" +
			"EndGlobal\r\n";

		private readonly Dictionary<string, Project> _projects = [];

		/// <summary>
		/// Gets or sets the solution path.
		/// </summary>
		public string SolutionPath { get; set; } = solutionPath;

		/// <summary>
		/// Gets the solution guid.
		/// </summary>
		public Guid Guid => solutionGuid;

		/// <summary>
		/// Adds a Visual Studio project to the solution.
		/// </summary>
		/// <param name="project">Project which should be added.</param>
		/// <param name="buildFileLocation">Location of the *.build.cs file</param>
		/// <returns>True if success, otherwise false.</returns>
		public bool AddProject(Project project, string buildFileLocation)
		{
			_projects[buildFileLocation] = project;

			return true;
		}

		/// <summary>
		/// Removes all projects from the solution.
		/// </summary>
		public void ClearAllProjects()
		{
			_projects.Clear();
		}

		/// <summary>
		/// Writes all projects and the solution file.
		/// </summary>
		/// <returns>True if success, otherwise false.</returns>
		public bool WriteFile()
		{
			ILogService? logService = ServiceProvider.LogService;

			DateTime startTime = DateTime.Now;

			logService?.Log($"Started at {startTime.ToLocalTime().ToShortTimeString()}");

			logService?.Log($"Solution : Generating");

			using (LogScope scope = new())
			{
				foreach (KeyValuePair<string, Project> project in _projects)
				{
					if (!IsProjectUpdateRequired(project))
					{
						continue;
					}

					project.Value.WriteFile();
					CacheProject(project);
				}

				Directory.CreateDirectory(Path.GetDirectoryName(SolutionPath) ?? "");

				IProfileService? profileService = ServiceProvider.ProfileService;
				IPlatformService? platformService = ServiceProvider.PlatformService;

				if (profileService == null || platformService == null)
				{
					logService?.LogError("Profile and platform are required.");
					logService?.LogError("Generating solution failed.");
					return false;
				}

				if (IsSolutionUpdateRequired(profileService, platformService))
				{
					string template = _solutionTemplate;

					WriteProjects(ref template, profileService, platformService);

					WritePlatforms(ref template, profileService, platformService);

					WriteNestedProjects(ref template, WriteFolders(ref template));

					template = template.Replace("%Visual.Studio.SolutionGuid%", $"{{{Guid}}}");
					File.WriteAllText(SolutionPath, template);

					CacheSolution(profileService, platformService);
				}
			}

			logService?.Log("Solution : Generating done");

			TimeSpan deltaTime = DateTime.Now - startTime;
			logService?.Log($"Completed at {startTime.ToLocalTime().ToShortTimeString()} and took {deltaTime}");
			return true;
		}

		/// <summary>
		/// Adds all folders to the solution.
		/// </summary>
		/// <param name="template">Current content of the template.</param>
		/// <returns>Returns a list of all folders.</returns>
		private List<Folder> WriteFolders(ref string template)
		{
			List<Folder> folders = [];
			Dictionary<string, int> folderTree = [];

			string foldersContent = "";

			foreach (KeyValuePair<string, Project> project in _projects)
			{
				IEnumerable<string> splittedPath = project.Value.RelativePath.Split("\\").Append(project.Value.Name);
				string curPath = "";
				Folder? activeFolder = null;

				foreach (string folder in splittedPath)
				{
					curPath += $"\\{folder}";

					if (folderTree.TryGetValue(curPath, out int index))
					{
						activeFolder = folders[index];
						continue;
					}
					else
					{
						Folder newFolder = new(folder, folder == project.Value.Name ? project.Value.Guid : Guid.NewGuid(), activeFolder, folder == project.Value.Name);

						folderTree.Add(curPath, folders.Count);
						folders.Add(newFolder);

						activeFolder = newFolder;
					}
				}
			}

			foreach (Folder registeredFolder in folders)
			{
				if (!registeredFolder.NestedOnly)
				{
					foldersContent += $"Project(\"{{{Project.ProjectToGuid(VisualStudioProjectType.Folder)}}}\") = \"{registeredFolder.Name}\", \"{registeredFolder.Name}\", \"{{{registeredFolder.Guid}}}\"\r\nEndProject\r\n";
				}
			}

			template = template.Replace("%Visual.Studio.Folders%", foldersContent);

			return folders;
		}

		/// <summary>
		/// Adds all projects to the solution.
		/// </summary>
		/// <param name="template">Current content of the template.</param>
		/// <param name="profileService">Profile Service of Luna.</param>
		/// <param name="platformService">Platform Service of Luna.</param>
		private void WriteProjects(ref string template, IProfileService profileService, IPlatformService platformService)
		{
			string projectsContent = "";
			string projectPlatformsContent = "";

			foreach (KeyValuePair<string, Project> project in _projects)
			{
				projectsContent += $"Project(\"{{{project.Value.TypeGuid}}}\") = \"{project.Value.Name}\", \"{project.Value.RelativePath}\\{project.Value.Name}\\{project.Value.Name}.{project.Value.Extension}\", \"{{{project.Value.Guid}}}\"\r\nEndProject\r\n";

				for (int curProfileIndex = 0; curProfileIndex < profileService?.GetProfileCount(); ++curProfileIndex)
				{
					string? profile = profileService?.GetProfileAt(curProfileIndex);

					for (int curPlatformIndex = 0; curPlatformIndex < platformService?.GetPlatformCount(); ++curPlatformIndex)
					{
						string? platform = platformService?.GetPlatformAt(curPlatformIndex);

						projectPlatformsContent += $"\t\t{{{project.Value.Guid}}}.{profile}|{platform}.ActiveCfg = {profile}|{platform}\r\n"
												+ $"\t\t{{{project.Value.Guid}}}.{profile}|{platform}.Build.0 = {profile}|{platform}\r\n";
					}
				}
			}

			template = template.Replace("%Visual.Studio.Projects%", projectsContent)
						.Replace("%Visual.Studio.ProjectPlatforms%", projectPlatformsContent);
		}

		/// <summary>
		/// Adds all platforms to the solution.
		/// </summary>
		/// <param name="template">Current content of the template.</param>
		/// <param name="profileService">Profile Service of Luna.</param>
		/// <param name="platformService">Platform Service of Luna.</param>
		private static void WritePlatforms(ref string template, IProfileService profileService, IPlatformService platformService)
		{
			string platformsContent = "";

			for (int curProfileIndex = 0; curProfileIndex < profileService?.GetProfileCount(); ++curProfileIndex)
			{
				string? profile = profileService?.GetProfileAt(curProfileIndex);

				for (int curPlatformIndex = 0; curPlatformIndex < platformService?.GetPlatformCount(); ++curPlatformIndex)
				{
					string? platform = platformService?.GetPlatformAt(curPlatformIndex);

					platformsContent += $"\t\t{profile}|{platform} = {profile}|{platform}\r\n";
				}
			}

			template = template.Replace("%Visual.Studio.Platforms%", platformsContent);
		}

		/// <summary>
		/// Adds the folder relations to the solution.
		/// </summary>
		/// <param name="template">Current content of the template.</param>
		/// <param name="folders">List of folders that were added to the solution.</param>
		private static void WriteNestedProjects(ref string template, List<Folder> folders)
		{
			string nestedProjectsContent = "";

			foreach (Folder folder in folders)
			{
				if (folder.Parent == null)
				{
					continue;
				}

				nestedProjectsContent += $"\t\t{{{folder.Guid}}} = {{{folder.Parent.Guid}}}\r\n";
			}

			template = template.Replace("%Visual.Studio.NestedProjects%", nestedProjectsContent);
		}

		/// <summary>
		/// Caches the solution.
		/// </summary>
		private void CacheSolution(IProfileService profileService, IPlatformService platformService)
		{
			string fullCachePath = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", SOLUTION_CACHE_FILE);

			List<string> profiles = [];
			List<string> platforms = [];

			int profileCount = profileService.GetProfileCount();
			for (int curIndex = 0; curIndex < profileCount; ++curIndex)
			{
				string? profile = profileService.GetProfileAt(curIndex);
				if (profile == null)
				{
					continue;
				}

				profiles.Add(profile);
			}

			int platformCount = platformService.GetPlatformCount();
			for (int curIndex = 0; curIndex < platformCount; ++curIndex)
			{
				string? platform = platformService.GetPlatformAt(curIndex);
				if (platform == null)
				{
					continue;
				}

				platforms.Add(platform);
			}

			SolutionCache cache = new(Guid, [.. _projects.Keys], platforms, profiles);

			string solutionJSON = JsonSerializer.Serialize(cache, typeof(SolutionCache), SolutionCacheSourceGenerationContext.Default);

			File.WriteAllText(fullCachePath, solutionJSON);
		}

		/// <summary>
		/// Checks if the solution needs to be updated.
		/// </summary>
		/// <returns>True if updated is needed, otherwise false.</returns>
		private bool IsSolutionUpdateRequired(IProfileService profileService, IPlatformService platformService)
		{
			string fullCachePath = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", SOLUTION_CACHE_FILE);

			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce == null)
			{
				return true;
			}

			if (!File.Exists(fullCachePath))
			{
				return true;
			}

			try
			{
				if (JsonSerializer.Deserialize(File.ReadAllText(fullCachePath), typeof(SolutionCache), SolutionCacheSourceGenerationContext.Default) is not SolutionCache cachedSolution)
				{
					logSerivce?.LogError($"Could not read cached solution.");
					return true;
				}

				if (Guid != cachedSolution.SolutionGuid // Guid doesn't match -> Update.
					|| _projects.Count != cachedSolution.Projects.Count // Project count mismatch -> Update.
					|| platformService.GetPlatformCount() != cachedSolution.Platforms.Count // Platform count changed -> Update.
					|| profileService.GetProfileCount() != cachedSolution.Profiles.Count) // Profiles count changed -> Update.
				{
					return true;
				}

				foreach (string buildFile in cachedSolution.Projects)
				{
					if (!_projects.TryGetValue(buildFile, out Project? project))
					{
						// A project changed -> Update.
						return true;
					}
				}

				foreach (string platform in cachedSolution.Platforms)
				{
					if (!platformService.HasPlatform(platform))
					{
						// Platform is no longer available -> Update.
						return true;
					}
				}

				foreach (string profile in cachedSolution.Profiles)
				{
					if (!profileService.HasProfile(profile))
					{
						// Profile is no longer available -> Update.
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				logSerivce?.LogError($"Could not read options cache.");
				logSerivce?.LogError($"{ex}");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Caches a project.
		/// </summary>
		/// <param name="project">Project which should be cached.</param>
		private void CacheProject(KeyValuePair<string, Project> project)
		{
			string cacheProjectPath = $"{project.Value.RelativePath}\\{project.Value.Name}\\{PROJECT_CACHE_FILE}";
			string fullProjectPath = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", cacheProjectPath);

			IOptionService? optionService = ServiceProvider.OptionService;

			Dictionary<Guid, bool> options = [];
			optionService?.VisitOptions((IOption option) =>
			{
				IFlagOption? flagOption = (IFlagOption?)option;
				if (flagOption == null)
				{
					return true;
				}

				options[option.Guid] = flagOption.IsEnabled;
				return true;
			});

			FileInfo fileInfo = new(project.Key);

			ProjectCache cache = new(project.Value.Guid, project.Key, fileInfo.LastWriteTime, options);

			string projectJSON = JsonSerializer.Serialize(cache, typeof(ProjectCache), ProjectCacheSourceGenerationContext.Default);

			File.WriteAllText(fullProjectPath, projectJSON);
		}

		/// <summary>
		/// Checks if a project file needs to be updated.
		/// </summary>
		/// <param name="project">Project that should be checked.</param>
		/// <returns>True if update is needed, otherwise false.</returns>
		private bool IsProjectUpdateRequired(KeyValuePair<string, Project> project)
		{
			string cacheProjectPath = $"{project.Value.RelativePath}\\{project.Value.Name}\\{PROJECT_CACHE_FILE}";
			string fullProjectPath = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", cacheProjectPath);

			ILogService? logSerivce = ServiceProvider.LogService;
			if (logSerivce == null)
			{
				return true;
			}

			if (!File.Exists(fullProjectPath))
			{
				return true;
			}

			try
			{
				if (JsonSerializer.Deserialize(File.ReadAllText(fullProjectPath), typeof(ProjectCache), ProjectCacheSourceGenerationContext.Default) is not ProjectCache cachedProject)
				{
					logSerivce?.LogError($"Could not read cached project.");
					return true;
				}

				FileInfo fileInfo = new(project.Key);

				if (project.Value.Guid != cachedProject.ProjectGuid // Guid doesn't match -> Update.
					|| project.Key != cachedProject.SourceCodeLocation // Build file location doesn't match -> Update.
					|| fileInfo.LastWriteTime != cachedProject.ModifiedDate) // Build file was modified -> Update.
				{
					return true;
				}

				IOptionService? optionService = ServiceProvider.OptionService;
				if (optionService == null)
				{
					return true;
				}

				foreach (KeyValuePair<Guid, bool> option in cachedProject.OptionsState)
				{
					bool? isEnabled = optionService?.IsOptionEnabled(option.Key);
					if (isEnabled == null)
					{
						// Option is missing -> Update.
						return true;
					}
					if (isEnabled != option.Value)
					{
						// Option value is different -> Update.
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				logSerivce?.LogError($"Could not read cached project.");
				logSerivce?.LogError($"{ex}");
				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// Serializer Context to add serialize support for project cache.
	/// </summary>
	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(ProjectCache))]
	internal partial class ProjectCacheSourceGenerationContext : JsonSerializerContext
	{
	}

	/// <summary>
	/// Serializer Context to add serialize support for solution cache.
	/// </summary>
	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(SolutionCache))]
	internal partial class SolutionCacheSourceGenerationContext : JsonSerializerContext
	{
	}
}