using Luna.Core;
using Luna.Core.Target;

namespace Luna.Targets.VisualStudio
{
	public class CWindowsTarget : ITarget
	{
		public string Name => "Visual Studio 2022 - Windows x64";

		public string SolutionFolder => "Windows";

		public string FullSolutionPath => Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), SolutionFolder);

		public void Register()
		{
			ServiceProvider.RegistryService.RegisterTarget(this);
		}

		public bool GenerateSolution()
		{
			Solution solution = new(Path.Combine(FullSolutionPath, $"{LunaConfig.Instance.Name}_{SolutionFolder}.sln"));

			int buildablesCount = ServiceProvider.RegistryService.GetBuildServiceCount();

			for (int curIndex = 0; curIndex < buildablesCount; ++curIndex)
			{
				IBuild? buildable = ServiceProvider.RegistryService.GetBuildServiceAt(curIndex);
				IProject? project = buildable?.Generate(solution);
				if (project != null)
				{
					solution.AddProject(project);
				}
			}

			return solution.WriteFile();
		}
	}
}