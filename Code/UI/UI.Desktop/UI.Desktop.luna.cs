using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;

namespace Luna.BuildScript.UI.Desktop
{
	/// <summary>
	/// Adds the UI desktop project.
	/// </summary>
	public class Builder : IBuild
	{
		/// <summary>
		/// Generates the project.
		/// </summary>
		/// <param name="solution">Solution to which the project should be added.</param>
		/// <returns>The project</returns>
		public IProject? Generate(ISolution solution)
		{
			bool? isUIEnabled = ServiceProvider.OptionService?.IsOptionEnabled(UI.Builder.OPTION_UI);
			if (!isUIEnabled.HasValue || !isUIEnabled.Value)
			{
				return null;
			}

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return null;
			}

			Project project = new(Project.ProjectToGuid(VisualStudioProjectType.CSharp), "UI.Desktop", "Luna\\UI", "5FFF8EE8-E148-461B-B409-A4377824E847".AsGuid(), (Solution)solution, ProjectService.ProjectExtension);

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.AvalonDesktopLauncher("app.manifest")
							.Files([
								new("app.manifest", "app.manifest", "None"),
								new("Program.cs", "Program.cs"),
							])
							.Reference([UI.Builder.NAME]);
			}

			return project;
		}

		/// <summary>
		/// Registers the ui desktop module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);
		}
	}
}