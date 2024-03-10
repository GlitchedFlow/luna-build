using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
using Luna.Targets.VisualStudio.Projects.CSharp;

namespace Luna.BuildScript.Targets.VisualStudio
{
	/// <summary>
	/// Adds Visual Studio target project
	/// </summary>
	public class Builder : IBuild
	{
		/// <summary>
		/// Configurates the options.
		/// </summary>
		public void Configurate()
		{
		}

		/// <summary>
		/// Generates the project.
		/// </summary>
		/// <param name="solution">Solution to which the project should be added.</param>
		/// <returns>The project</returns>
		public IProject? Generate(ISolution solution)
		{
			Project project = new("VisualStudio", "Luna\\Targets", "5A027767-8D3A-47B4-8AC7-F444E877BFBA".AsGuid(), (Solution)solution);

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return project;
			}

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.Target()
							.Files([
								new("Projects\\Cpp\\Project.cs", "Projects\\Cpp\\Project.cs"),

								new("Projects\\CSharp\\Project.cs", "Projects\\CSharp\\Project.cs"),

								new("Projects\\BaseProject.cs", "Projects\\BaseProject.cs"),

								new("Solution.cs", "Solution.cs"),
								new("Target.cs", "Target.cs"),
							])
							.Reference([Core.Builder.NAME]);
			}

			return project;
		}

		/// <summary>
		/// Registers the visual studio target module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);
		}
	}
}