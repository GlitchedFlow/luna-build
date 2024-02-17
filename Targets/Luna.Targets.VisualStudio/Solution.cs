using Luna.Core.Target;

namespace Luna.Targets.VisualStudio
{
	public class CSolution(string solutionPath) : ISolution
	{
		public string SolutionPath { get; } = solutionPath;

		public IProject AddProject()
		{
			throw new NotImplementedException();
		}
	}
}
