using Luna.BuildScript.Meta;
using Luna.BuildScript.CLI;
using Luna.BuildScript.Core;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;

namespace Luna.BuildScript.UI
{
	/// <summary>
	/// Adds the core UI project.
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "UI.Core";
		public static Guid OPTION_UI = "{D4A4F086-29A2-43BD-AA30-64CE798DEFC2}".AsGuid();

		private IFlagOption? _option = null;

		/// <summary>
		/// Generates the project.
		/// </summary>
		/// <param name="solution">Solution to which the project should be added.</param>
		/// <returns>The project</returns>
		public IProject? Generate(ISolution solution)
		{
			if (_option == null || !_option.IsEnabled)
			{
				return null;
			}

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return null;
			}

			Project project = new(Project.ProjectToGuid(VisualStudioProjectType.CSharp), NAME, "Luna\\UI", "61F643FF-D785-4914-A239-5C115C211604".AsGuid(), (Solution)solution, ProjectService.ProjectExtension);

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.AvalonLauncher()
							.Files([
								new("Helpers\\Entry.cs", "Helpers\\Entry.cs"),
								new("Helpers\\OptionGroup.cs", "Helpers\\OptionGroup.cs"),
								new("Helpers\\OptionTemplateSelector.cs", "Helpers\\OptionTemplateSelector.cs"),

								new("ViewModels\\MainViewModel.cs", "ViewModels\\MainViewModel.cs"),
								new("ViewModels\\ViewModelBase.cs", "ViewModels\\ViewModelBase.cs"),

								new("Views\\OptionView.axaml", "Views\\OptionView.axaml", "AvaloniaXaml"),
								new("Views\\OptionView.axaml.cs", "Views\\OptionView.axaml.cs"),
								new("Views\\SetupView.axaml", "Views\\SetupView.axaml", "AvaloniaXaml"),
								new("Views\\SetupView.axaml.cs", "Views\\SetupView.axaml.cs"),
								new("Views\\MainWindow.axaml", "Views\\MainWindow.axaml", "AvaloniaXaml"),
								new("Views\\MainWindow.axaml.cs", "Views\\MainWindow.axaml.cs"),

								new("App.axaml", "App.axaml", "AvaloniaXaml"),
								new("App.axaml.cs", "App.axaml.cs"),
							])
							.Reference([CLI.Builder.NAME, Core.Builder.NAME]);
			}

			return project;
		}

		/// <summary>
		/// Registers the UI core module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);

			_option = ServiceProvider.OptionService?.RegisterFlagOption(OPTION_UI, "UI", true);
		}
	}
}