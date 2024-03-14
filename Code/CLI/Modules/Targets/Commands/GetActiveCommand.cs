using Luna.Core;

namespace Luna.CLI.Modules.Targets
{
	public class GetActiveCommand : BaseCommand
	{
		public override string Name => "get";

		public override string Description => "Gets the active target";

		public override void Execute(string[] args)
		{
			ILogService? logSerivce = ServiceProvider.LogService;
			IGeneratorService? generatorService = ServiceProvider.GeneratorService;

			if (logSerivce != null && generatorService != null)
			{
				logSerivce.Log($"Active Target is set to {(generatorService.ActiveTarget != null ? generatorService.ActiveTarget.Name : "[None]")}");
			}
		}
	}
}