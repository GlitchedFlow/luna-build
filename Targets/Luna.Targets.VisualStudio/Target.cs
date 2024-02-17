using Luna.Core;
using Luna.Core.Target;

namespace Luna.Targets.VisualStudio
{
    public class CWindowsTarget : ITarget
	{
		public string Name => "Visual Studio 2022 - Windows x64";

		public string SolutionPath { get; set; } = "";

		public void Register()
		{
			CServiceProvider.RegistryService.RegisterTarget(this);
		}

		public void GenerateSolution()
		{

		}
	}

	public class CXboxTarget : ITarget
	{
		public string Name => "Visual Studio 2022 - Xbox Series";

		public string SolutionPath { get; set; } = "";

		public void Register()
		{
			CServiceProvider.RegistryService.RegisterTarget(this);
		}

		public void GenerateSolution()
		{
			CSolution solution = new(SolutionPath);

			int buildablesCount = CServiceProvider.RegistryService.GetBuildServiceCount();
			for (int i = 0; i < buildablesCount; ++i)
			{
				IBuild? buildable = CServiceProvider.RegistryService.GetBuildServiceAt(i);
				if (buildable != null)
				{
					buildable.Build(solution);
				}
			}
		}
	}
}
