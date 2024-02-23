namespace Luna.Core.Target
{
	/// <summary>
	/// Generic interface that describes a solution.
	/// </summary>
	public interface ISolution : IIdentifier
	{
		/// <summary>
		/// Writes the solution file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool WriteFile();
	}
}