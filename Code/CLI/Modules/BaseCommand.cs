using Luna.Core;

namespace Luna.CLI
{
	/// <summary>
	/// Base class for all commands.
	/// </summary>
	public abstract class BaseCommand
	{
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="args">Arguments for the command.</param>
		/// <returns>True if successful, otherwise false.</returns>
		public abstract bool Execute(string[] args);

		/// <summary>
		/// Prints the help for this command.
		/// </summary>
		public void Help()
		{
			ILogService? logService = ServiceProvider.LogService;
			if (logService == null)
			{
				return;
			}

			logService.Log($"{Name} - {Description}");
		}
	}
}