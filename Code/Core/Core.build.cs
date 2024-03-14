using Luna.BuildScript.Meta;
using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
using Luna.Targets.VisualStudio.Projects.CSharp;

namespace Luna.BuildScript.Core
{
	/// <summary>
	/// Adds the core project
	/// </summary>
	public class Builder : IBuild
	{
		public const string NAME = "Core";

		/// <summary>
		/// Generates the project.
		/// </summary>
		/// <param name="solution">Solution to which the project should be added.</param>
		/// <returns>The project</returns>
		public IProject? Generate(ISolution solution)
		{
			Project project = new(NAME, "Luna", "D623DB34-0FC2-4858-84BD-676B497943B6".AsGuid(), (Solution)solution);

			ProjectService? projectService = ServiceProvider.RegistryService.GetMetaService<ProjectService>();
			if (projectService == null)
			{
				return project;
			}

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.StandaloneLibrary()
							.Files([
								new("Bridge\\ArgumentParser.cs", "Bridge\\ArgumentParser.cs"),
								new("Bridge\\Compiler.cs", "Bridge\\Compiler.cs"),
								new("Bridge\\Kickstart.cs", "Bridge\\Kickstart.cs"),
								new("Bridge\\LunaConfig.cs", "Bridge\\LunaConfig.cs"),

								new("Private\\Cache.cs", "Private\\Cache.cs"),
								new("Private\\GeneratorService.cs", "Private\\GeneratorService.cs"),
								new("Private\\Identifier.cs", "Private\\Identifier.cs"),
								new("Private\\LogService.cs", "Private\\LogService.cs"),
								new("Private\\OptionService.cs", "Private\\OptionService.cs"),
								new("Private\\PlatformService.cs", "Private\\PlatformService.cs"),
								new("Private\\ProfileService.cs", "Private\\ProfileService.cs"),
								new("Private\\RegistryService.cs", "Private\\RegistryService.cs"),

								new("Public\\Target\\IProject.cs", "Public\\Target\\IProject.cs"),
								new("Public\\Target\\ISolution.cs", "Public\\Target\\ISolution.cs"),
								new("Public\\Target\\ITarget.cs", "Public\\Target\\ITarget.cs"),

								new("Public\\GuidExtension.cs", "Public\\GuidExtension.cs"),
								new("Public\\IBuild.cs", "Public\\IBuild.cs"),
								new("Public\\IGeneratorService.cs", "Public\\IGeneratorService.cs"),
								new("Public\\IIdentifier.cs", "Public\\IIdentifier.cs"),
								new("Public\\ILogService.cs", "Public\\ILogService.cs"),
								new("Public\\IMeta.cs", "Public\\IMeta.cs"),
								new("Public\\IOptionService.cs", "Public\\IOptionService.cs"),
								new("Public\\IPlatformService.cs", "Public\\IPlatformService.cs"),
								new("Public\\IProfileService.cs", "Public\\IProfileService.cs"),
								new("Public\\IRegistryService.cs", "Public\\IRegistryService.cs")
							]);
			}

			return project;
		}

		/// <summary>
		/// Registers the Core module.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterBuildService(this);
		}
	}
}