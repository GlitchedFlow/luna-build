// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio;
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

			Project project = new(Project.ProjectToGuid(VisualStudioProjectType.CSharp), NAME, "Luna", "5113F5B2-168E-4CED-89A4-016C235EFAE9".AsGuid(), (Solution)solution, ProjectService.ProjectExtension);

			using (ProjectService.Scope scope = new(project.ProjectRoot))
			{
				projectService.ConsoleLauncher()
							.Reference([Core.Builder.NAME])
							.Files([
								new("Console.cs", "Console.cs"),

								new("Modules\\BaseCommand.cs", "Modules\\BaseCommand.cs"),
								new("Modules\\BaseModule.cs", "Modules\\BaseModule.cs"),

								new("Modules\\Core\\CoreModule.cs", "Modules\\Core\\CoreModule.cs"),
								new("Modules\\Core\\Commands\\ListCommand.cs", "Modules\\Core\\Commands\\ListCommand.cs"),
								new("Modules\\Core\\Commands\\SwitchCommand.cs", "Modules\\Core\\Commands\\SwitchCommand.cs"),
								new("Modules\\Core\\Commands\\GetCommand.cs", "Modules\\Core\\Commands\\GetCommand.cs"),
								new("Modules\\Core\\Commands\\SetCommand.cs", "Modules\\Core\\Commands\\SetCommand.cs"),
								new("Modules\\Core\\Commands\\CompileCommand.cs", "Modules\\Core\\Commands\\CompileCommand.cs"),
								new("Modules\\Core\\Commands\\InitCommand.cs", "Modules\\Core\\Commands\\InitCommand.cs"),
								new("Modules\\Core\\Commands\\LoadCommand.cs", "Modules\\Core\\Commands\\LoadCommand.cs"),

								new("Modules\\Generator\\GeneratorModule.cs", "Modules\\Generator\\GeneratorModule.cs"),
								new("Modules\\Generator\\Commands\\GenerateCommand.cs", "Modules\\Generator\\Commands\\GenerateCommand.cs"),

								new("Modules\\Options\\OptionsModule.cs", "Modules\\Options\\OptionsModule.cs"),
								new("Modules\\Options\\Commands\\ListCommand.cs", "Modules\\Options\\Commands\\ListCommand.cs"),
								new("Modules\\Options\\Commands\\SetOptionCommand.cs", "Modules\\Options\\Commands\\SetOptionCommand.cs"),

								new("Modules\\Plugins\\PluginsModule.cs", "Modules\\Plugins\\PluginsModule.cs"),
								new("Modules\\Plugins\\Commands\\ListCommand.cs", "Modules\\Plugins\\Commands\\ListCommand.cs"),

								new("Modules\\Targets\\TargetModule.cs", "Modules\\Targets\\TargetModule.cs"),
								new("Modules\\Targets\\Commands\\GetActiveCommand.cs", "Modules\\Targets\\Commands\\GetActiveCommand.cs"),
								new("Modules\\Targets\\Commands\\ListCommand.cs", "Modules\\Targets\\Commands\\ListCommand.cs"),
								new("Modules\\Targets\\Commands\\SetActiveCommand.cs", "Modules\\Targets\\Commands\\SetActiveCommand.cs")
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