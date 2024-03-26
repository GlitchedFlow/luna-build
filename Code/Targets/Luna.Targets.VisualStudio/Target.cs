using Luna.Core;
using Luna.Core.Target;

namespace Luna.Targets.VisualStudio
{
	/// <summary>
	/// Visual Studio 2022 - Win x64 Provider class
	/// </summary>
	public class CWindowsTarget : ITarget
	{
		private readonly Guid _windowsGuid = "{69838B76-984D-41BD-AEF5-8FF18E5C9620}".AsGuid();

		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public string Name => "Visual Studio 2022 - Windows x64";

		/// <summary>
		/// Gets the solution folder of the target.
		/// </summary>
		public string SolutionFolder => "Windows";

		/// <summary>
		/// Gets the full solution path.
		/// </summary>
		public string FullSolutionPath => Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), SolutionFolder);

		/// <summary>
		/// Gets or sets the solution for this target.
		/// </summary>
		public Solution Solution { get; set; } = new("", Guid.Empty);

		/// <summary>
		/// Registers the target with luna.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterTarget(this);

			// Set default solution.
			Solution = new(Path.Combine(FullSolutionPath, $"{LunaConfig.Instance.Name}_{SolutionFolder}.sln"), _windowsGuid);
		}

		/// <summary>
		/// Generates the solution.
		/// </summary>
		/// <returns>True if success, otherwise false.</returns>
		public bool GenerateSolution()
		{
			// Update solution path.
			Solution.SolutionPath = Path.Combine(FullSolutionPath, $"{LunaConfig.Instance.Name}_{SolutionFolder}.sln");

			Solution.ClearAllProjects();

			int buildablesCount = ServiceProvider.RegistryService.GetBuildServiceCount();

			for (int curIndex = 0; curIndex < buildablesCount; ++curIndex)
			{
				IBuild? buildable = ServiceProvider.RegistryService.GetBuildServiceAt(curIndex);
				if (buildable == null)
				{
					continue;
				}

				string? sourceLocation = ServiceProvider.RegistryService.GetSourceCodeLocation(buildable);
				if (sourceLocation == null)
				{
					continue;
				}

				Project? project = (Project?)buildable?.Generate(Solution);
				if (project == null)
				{
					continue;
				}

				Solution.AddProject(project, sourceLocation);
			}

			return Solution.WriteFile();
		}
	}
}