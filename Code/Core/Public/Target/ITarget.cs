namespace Luna.Core.Target
{
	/// <summary>
	/// Generic interface to describe a target.
	/// </summary>
	public interface ITarget : ILuna
	{
		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the name of the solution folder.
		/// </summary>
		string SolutionFolder { get; }

		/// <summary>
		/// Gets the full solution path.
		/// </summary>
		string FullSolutionPath { get; }

		/// <summary>
		/// Registers the target. Called by the system.
		/// </summary>
		void Register();

		/// <summary>
		/// Generates the solution.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool GenerateSolution();
	}
}