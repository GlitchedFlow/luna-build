using Luna.Core.Target;
using Luna.Core;

namespace Luna.Targets.VisualStudio.ProjectGenerators.Cpp
{
	/// <summary>
	/// Generator class for cpp prpjects.
	/// </summary>
	public class Generator : IMeta
	{
		/// <summary>
		/// Registers the project. Called by the system.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService(this);
		}

		public IProject CreateProject()
		{
			return new Project();
		}
	}
}