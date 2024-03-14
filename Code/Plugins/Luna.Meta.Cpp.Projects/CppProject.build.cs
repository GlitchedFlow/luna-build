using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
using Luna.Targets.VisualStudio.Projects.CSharp;

namespace Luna.BuildScript.Plugins.CppProject
{
	/// <summary>
	/// Adds the CppProject plugin project
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "CppProject";

		private IFlagOption? _option = null;

		public static Guid CPP_PROJECT = "{2CDC5CA5-4FF3-4784-B7C6-15FAC58C712E}".AsGuid();

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

			Project project = new(NAME, "LunaPlugins", "{8F44E5DF-9A93-4561-8AE9-C337E69BF3CD}".AsGuid(), (Solution)solution);

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return project;
			}

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.Plugin()
							.Files([
								new("GeneratorService.cs", "GeneratorService.cs")
							])
							.Reference([Core.Builder.NAME]);
			}

			return project;
		}

		/// <summary>
		/// Registers the plugin module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);

			_option = ServiceProvider.OptionService?.RegisterFlagOption(CPP_PROJECT, "Cpp Projects", true, "Plugins");
		}
	}
}