using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Generic interface that describes a build service.
	/// </summary>
	public interface IBuild : ILuna
	{
		/// <summary>
		/// Registers the build services. Called by system.
		/// </summary>
		void Register();

		/// <summary>
		/// Generate the project.
		/// </summary>
		/// <param name="solution">The solution to which this project will be added.</param>
		/// <returns>Valid project if successful, otherwise false.</returns>
		IProject? Generate(ISolution solution);
	}
}