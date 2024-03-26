using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;

namespace Luna.BuildScript.CLI.Plugins.Sample
{
	/// <summary>
	/// Adds the Sample CLI plugin project
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "CLI Sample";

		private IFlagOption? _option = null;

		public static Guid CLI_SAMPLE_PROJECT = "{A0D25303-529E-4D0F-B4B2-DFCCBEE8E594}".AsGuid();

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

			Project project = new(Project.ProjectToGuid(VisualStudioProjectType.CSharp), NAME, "LunaPlugins\\CLI", "{08EC9B0F-F376-4EB5-ADDF-2AD2200076DC}".AsGuid(), (Solution)solution, ProjectService.ProjectExtension);

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.CLIPlugin()
							.Files([
								new("SamplePlugin.cs", "SamplePlugin.cs")
							])
							.Reference([Core.Builder.NAME, CLI.Builder.NAME]);
			}

			return project;
		}

		/// <summary>
		/// Registers the plugin module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);

			_option = ServiceProvider.OptionService?.RegisterFlagOption(CLI_SAMPLE_PROJECT, "Sample Project", true, "CLI Plugins");
		}
	}
}