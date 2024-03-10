using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
using Luna.Targets.VisualStudio.Projects.CSharp;

namespace Luna.BuildScript.UI
{
	/// <summary>
	/// Adds the core UI project.
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "UI.Core";

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
			Project project = new(NAME, "Luna\\UI", "61F643FF-D785-4914-A239-5C115C211604".AsGuid(), (Solution)solution);

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return project;
			}

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.AvalonLauncher()
							.Files([
								new("ViewModels\\MainViewModel.cs", "ViewModels\\MainViewModel.cs"),
								new("ViewModels\\ViewModelBase.cs", "ViewModels\\ViewModelBase.cs"),

								new("Views\\MainView.axaml", "Views\\MainView.axaml", "AvaloniaXaml"),
								new("Views\\MainView.axaml.cs", "Views\\MainView.axaml.cs"),
								new("Views\\MainWindow.axaml", "Views\\MainWindow.axaml", "AvaloniaXaml"),
								new("Views\\MainWindow.axaml.cs", "Views\\MainWindow.axaml.cs"),

								new("App.axaml", "App.axaml", "AvaloniaXaml"),
								new("App.axaml.cs", "App.axaml.cs"),
							]);
			}

			return project;
		}

		/// <summary>
		/// Registers the UI core module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);
		}
	}
}