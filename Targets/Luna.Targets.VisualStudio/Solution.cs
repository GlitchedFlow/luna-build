using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio.Projects;

namespace Luna.Targets.VisualStudio
{
	public class Solution(string solutionPath, Guid solutionGuid) : ISolution
	{
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

		private readonly List<BaseProject> _projects = [];

		public string SolutionPath => solutionPath;

		public Guid Guid => solutionGuid;

		public bool AddProject(BaseProject project)
		{
			_projects.Add(project);

			return true;
		}

		public bool WriteFile()
		{
			ILogService? logService = ServiceProvider.LogService;

			logService?.Log($"Generating solution...");

			foreach (BaseProject project in _projects)
			{
				project.WriteFile();
			}

			Directory.CreateDirectory(Path.GetDirectoryName(SolutionPath));

			IProfileService? profileService = ServiceProvider.ProfileService;
			IPlatformService? platformService = ServiceProvider.PlatformService;

			if (profileService == null || platformService == null)
			{
				logService?.LogError("Profile and platform are required.");
				return false;
			}

			string template = _solutionTemplate;

			WriteProjects(ref template, profileService, platformService);

			WritePlatforms(ref template, profileService, platformService);

			WriteNestedProjects(ref template, WriteFolders(ref template));

			template = template.Replace("%Visual.Studio.SolutionGuid%", $"{{{Guid}}}");
			File.WriteAllText(SolutionPath, template);

			logService?.Log($"Generating solution done.");
			return true;
		}

		private List<Folder> WriteFolders(ref string template)
		{
			List<Folder> folders = [];
			Dictionary<string, int> folderTree = [];

			string foldersContent = "";

			foreach (BaseProject project in _projects)
			{
				IEnumerable<string> splittedPath = project.RelativePath.Split("\\").Append(project.Name);
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
						Folder newFolder = new(folder, folder == project.Name ? project.Guid : Guid.NewGuid(), activeFolder, folder == project.Name);

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
					foldersContent += $"Project(\"{{{BaseProject.ProjectToGuid(VisualStudioProjectType.Folder)}}}\") = \"{registeredFolder.Name}\", \"{registeredFolder.Name}\", \"{{{registeredFolder.Guid}}}\"\r\nEndProject\r\n";
				}
			}

			template = template.Replace("%Visual.Studio.Folders%", foldersContent);

			return folders;
		}

		private void WriteProjects(ref string template, IProfileService profileService, IPlatformService platformService)
		{
			string projectsContent = "";
			string projectPlatformsContent = "";

			foreach (BaseProject project in _projects)
			{
				projectsContent += $"Project(\"{{{project.TypeGuid}}}\") = \"{project.Name}\", \"{project.RelativePath}\\{project.Name}\\{project.Name}.{project.Extension}\", \"{{{project.Guid}}}\"\r\nEndProject\r\n";

				for (int curProfileIndex = 0; curProfileIndex < profileService?.GetProfileCount(); ++curProfileIndex)
				{
					string? profile = profileService?.GetProfileAt(curProfileIndex);

					for (int curPlatformIndex = 0; curPlatformIndex < platformService?.GetPlatformCount(); ++curPlatformIndex)
					{
						string? platform = platformService?.GetPlatformAt(curPlatformIndex);

						projectPlatformsContent += $"\t\t{{{project.Guid}}}.{profile}|{platform}.ActiveCfg = {profile}|{platform}\r\n"
												+ $"\t\t{{{project.Guid}}}.{profile}|{platform}.Build.0 = {profile}|{platform}\r\n";
					}
				}
			}

			template = template.Replace("%Visual.Studio.Projects%", projectsContent)
						.Replace("%Visual.Studio.ProjectPlatforms%", projectPlatformsContent);
		}

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
	}
}