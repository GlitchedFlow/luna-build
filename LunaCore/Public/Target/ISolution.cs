namespace Luna.Core.Target
{
	/// <summary>
	/// Generic interface that describes a solution.
	/// </summary>
	public interface ISolution
	{
		/// <summary>
		/// Adds a project to the solution.
		/// </summary>
		/// <param name="project">The project that should be added.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public bool AddProject(IProject project);

		/// <summary>
		/// Writes the solution file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		public bool WriteFile();
	}
}