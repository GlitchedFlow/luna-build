namespace Luna.Core.Target
{
	/// <summary>
	/// Generic interface that describes a project.
	/// </summary>
	public interface IProject : IIdentifier
	{
		/// <summary>
		/// Writes the project file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool WriteFile();
	}
}