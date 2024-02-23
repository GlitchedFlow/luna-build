using Luna.Core;
using Luna.Core.Target;
using Luna.Targets.VisualStudio.Projects;

namespace Luna.Targets.VisualStudio
{
	public class CWindowsTarget : ITarget
	{
		private readonly Guid _windowsGuid = "{69838B76-984D-41BD-AEF5-8FF18E5C9620}".AsGuid();

		public string Name => "Visual Studio 2022 - Windows x64";

		public string SolutionFolder => "Windows";

		public string FullSolutionPath => Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), SolutionFolder);

		public void Register()
		{
			ServiceProvider.RegistryService.RegisterTarget(this);
		}

		public bool GenerateSolution()
		{
			Solution solution = new(Path.Combine(FullSolutionPath, $"{LunaConfig.Instance.Name}_{SolutionFolder}.sln"), _windowsGuid);

			int buildablesCount = ServiceProvider.RegistryService.GetBuildServiceCount();

			for (int curIndex = 0; curIndex < buildablesCount; ++curIndex)
			{
				IBuild? buildable = ServiceProvider.RegistryService.GetBuildServiceAt(curIndex);
				BaseProject? project = (BaseProject?)buildable?.Generate(solution);
				if (project == null)
				{
					continue;
				}

				solution.AddProject(project);
			}

			return solution.WriteFile();
		}
	}
}