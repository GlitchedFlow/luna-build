using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
using Luna.Targets.VisualStudio.Projects.CSharp;
using Luna.BuildScript.Meta;

namespace Luna.BuildScript.CLI
{
	/// <summary>
	/// Adds the CLI as a project.
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "CLI";

		/// <summary>
		/// Configurates the CLI options.
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
			Project project = new(NAME, "Luna", "5113F5B2-168E-4CED-89A4-016C235EFAE9".AsGuid(), (Solution)solution);

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return project;
			}

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.ConsoleLauncher()
							.Reference([Core.Builder.NAME])
							.Files([
								new("Console.cs", "Console.cs")
							]);
			}

			return project;
		}

		/// <summary>
		/// Registers the CLI module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);
		}
	}
}