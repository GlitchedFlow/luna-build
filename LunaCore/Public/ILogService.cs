namespace Luna.Core
{
	/// <summary>
	/// Base interface for the log service of luna.
	/// </summary>
	public interface ILogService : IMeta
	{
		/// <summary>
		/// Gets or sets if logging to console is enabled.
		/// </summary>
		bool IsLogToConsoleEnabled { get; set; }

		/// <summary>
		/// Gets or sets if logging to file is enabled.
		/// </summary>
		bool IsLogToFileEnabled { get; set; }

		/// <summary>
		/// Gets or sets if timestamps are enabled.
		/// </summary>
		bool IsTimestampEnabled { get; set; }

		/// <summary>
		/// Gets or sets the indentation style of scopes.
		/// </summary>
		string Indentation { get; set; }

		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <param name="content">Content of the message.</param>
		/// <param name="foreGround">Foreground color for console.</param>
		/// <param name="backGround">Background color for console.</param>
		public void Log(string content, ConsoleColor foreGround = ConsoleColor.White, ConsoleColor backGround = ConsoleColor.Black);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="content">Content of the info.</param>
		public void LogInfo(string content);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="content">Content of the warning.</param>
		public void LogWarning(string content);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="content">Content of the error.</param>
		public void LogError(string content);

		/// <summary>
		/// Logs a success message.
		/// </summary>
		/// <param name="content">Content of the success.</param>
		public void LogSuccess(string content);

		/// <summary>
		/// Opens a new scope.
		/// </summary>
		public void OpenScope();

		/// <summary>
		/// Closes the current scope.
		/// </summary>
		public void CloseScope();
	}
}