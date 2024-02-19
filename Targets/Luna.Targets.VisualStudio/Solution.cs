using Luna.Core.Target;

namespace Luna.Targets.VisualStudio
{
	public class Solution(string solutionPath) : ISolution
	{
		private readonly List<IProject> _projects = [];

		public string SolutionPath { get; } = solutionPath;

		public bool AddProject(IProject project)
		{
			_projects.Add(project);

			return true;
		}

		public bool WriteFile()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(SolutionPath));
			File.WriteAllText(SolutionPath, "");
			return true;
		}
	}
}