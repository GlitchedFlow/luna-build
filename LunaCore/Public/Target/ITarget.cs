namespace Luna.Core.Target
{
	public interface ITarget
	{
		string Name { get; }

		string SolutionFolder { get; }

		string FullSolutionPath { get; }

		void Register();

		bool GenerateSolution();
	}
}