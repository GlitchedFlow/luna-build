using Luna.Core;

namespace Luna.Targets.VisualStudio.Projects.Cpp
{
	public class Project(string name, string relativePath, Guid guid, Solution solution)
		: BaseProject(BaseProject.ProjectToGuid(VisualStudioProjectType.Cpp), name, relativePath, guid, solution)
	{
		public override string Extension => "vcxproj";

		public override bool WriteFile()
		{
			ILogService? logService = ServiceProvider.LogService;

			string projectDir = $"{RelativePath}\\{Name}\\";
			string projectFile = $"{projectDir}{Name}.{Extension}";
			logService?.Log($"Writing Project: {projectFile}");
			ProjectEntry someEntry = new("Project", "", [], new Dictionary<string, string>()
			{
				{ "DefaultTargets", "Build" },
				{ "ToolsVersion", "16.0"}
			});

			string fileBuffer = "";
			WriteTreeToBuffer(someEntry, ref fileBuffer);

			string fullProjectDirPath = Path.Combine(Path.GetDirectoryName(Solution.SolutionPath), projectDir);
			string fullProjectFilePath = Path.Combine(Path.GetDirectoryName(Solution.SolutionPath), projectFile);
			if (!Directory.Exists(Path.GetDirectoryName(fullProjectDirPath)))
			{
				Directory.CreateDirectory(fullProjectDirPath);
			}

			File.WriteAllText(fullProjectFilePath, fileBuffer);

			logService?.Log($"Writing Project Done.");
			return true;
		}
	}
}