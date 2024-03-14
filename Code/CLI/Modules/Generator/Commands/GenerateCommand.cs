using Luna.Core;

namespace Luna.CLI.Modules.Generator
{
	public class GenerateCommand : BaseCommand
	{
		public override string Name => "generate";

		public override string Description => "Generates the solution";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			IGeneratorService? generatorService = ServiceProvider.GeneratorService;
			if (logSerivce != null && generatorService != null)
			{
				if (generatorService.ActiveTarget == null)
				{
					logSerivce.LogError($"No active target was set.");
				}

				if (!generatorService.Generate())
				{
					logSerivce.LogError("Solution was not generated.");
				}
				else
				{
					logSerivce.LogSuccess("Solution was generated.");
				}
			}
		}
	}
}