using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;

namespace Luna.BuildScript.Targets.VisualStudio
{
	/// <summary>
	/// Adds Visual Studio target project
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "VisualStudio";

		/// <summary>
		/// Generates the project.
		/// </summary>
		/// <param name="solution">Solution to which the project should be added.</param>
		/// <returns>The project</returns>
		public IProject? Generate(ISolution solution)
		{
			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return null;
			}

			Project project = new(Project.ProjectToGuid(VisualStudioProjectType.CSharp), NAME, "Luna\\Targets", "5A027767-8D3A-47B4-8AC7-F444E877BFBA".AsGuid(), (Solution)solution, ProjectService.ProjectExtension);

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.Target()
							.Files([
								new("Project.cs", "Project.cs"),
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