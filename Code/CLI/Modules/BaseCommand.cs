using Luna.Core;

namespace Luna.CLI
{
	public abstract class BaseCommand
	{
		public abstract string Name { get; }
		public abstract string Description { get; }

		public abstract void Execute(string[] args);

		public void Help()
		{
			ILogService? logService = ServiceProvider.LogService;
			if (logService != null)
			{
				logService.Log($"{Name} - {Description}");
			}
		}
	}
}