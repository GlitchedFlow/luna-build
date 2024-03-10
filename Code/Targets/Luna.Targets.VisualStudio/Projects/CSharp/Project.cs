using Luna.Core;

namespace Luna.Targets.VisualStudio.Projects.CSharp
{
	public class Project(string name, string relativePath, Guid guid, Solution solution)
		: BaseProject(BaseProject.ProjectToGuid(VisualStudioProjectType.CSharp), name, relativePath, guid, solution)
	{
		private readonly ProjectEntry _projectRoot = new("Project", "", [], []);

		public override string Extension => "csproj";

		public ProjectEntry ProjectRoot => _projectRoot;

		public override bool WriteFile()
		{
			ILogService? logService = ServiceProvider.LogService;

			string projectDir = $"{RelativePath}\\{Name}\\";
			string projectFile = $"{projectDir}{Name}.{Extension}";
			logService?.Log($"Writing Project: {projectFile}");

			string fileBuffer = "";
			WriteTreeToBuffer(ProjectRoot, ref fileBuffer);

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