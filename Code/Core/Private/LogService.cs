using System;

namespace Luna.Core
{
	/// <summary>
	/// Core service class that is used to handle logging.
	/// </summary>
	internal class LogService : ILogService
	{
		private static LogService _instance = new();

		private int _indentationLevel = 0;
		private string _cachedFileContent = "";
		private const string _logFileName = "luna.log";

		/// <summary>
		/// Gets or sets if logging to console is enabled.
		/// </summary>
		public bool IsLogToConsoleEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets if logging to file is enabled.
		/// </summary>
		public bool IsLogToFileEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets if timestamp should be added to each entry.
		/// </summary>
		public bool IsTimestampEnabled { get; set; } = true;

		/// <summary>
		/// Gets or sets the indetation style for the log.
		/// </summary>
		public string Indentation { get; set; } = "  ";

		/// <summary>
		/// Gets the singleton instance for this service.
		/// </summary>
		internal static LogService Instance => _instance;

		/// <summary>
		/// Registers the service. Called by system.
		/// </summary>
		public void Register()
		{
			RegistryService.Instance.RegisterMetaService((ILogService)_instance);
		}

		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <param name="content">Content of the message.</param>
		/// <param name="foreGround">Foreground color for the console.</param>
		/// <param name="backGround">Background color for the console.</param>
		public void Log(string content, ConsoleColor foreGround = ConsoleColor.White, ConsoleColor backGround = ConsoleColor.Black)
		{
			string formatted = IsTimestampEnabled ? $"<{DateTime.Now.ToLongTimeString()}> " : "";
			formatted += $"{Prefix()}{content}";

			if (IsLogToConsoleEnabled)
			{
				Console.ForegroundColor = foreGround;
				Console.BackgroundColor = backGround;

				Console.WriteLine(formatted);

				Console.ResetColor();
			}
			if (IsLogToFileEnabled)
			{
				_cachedFileContent += $"{formatted}\r\n";

				if (LunaConfig.Instance)
				{
					File.WriteAllText(Path.Combine(Cache.GetCacheFolder(), _logFileName), _cachedFileContent);
				}
			}
		}

		/// <summary>
		/// Logs message as info.
		/// </summary>
		/// <param name="content">Content of the info.</param>
		public void LogInfo(string content)
		{
			Log($"[INFO] {content}", ConsoleColor.Blue);
		}

		/// <summary>
		/// Logs message as warning.
		/// </summary>
		/// <param name="content">Content of the warning.</param>
		public void LogWarning(string content)
		{
			Log($"[WARNING] {content}", ConsoleColor.Yellow);
		}

		/// <summary>
		/// Logs message as error.
		/// </summary>
		/// <param name="content">Content of the error.</param>
		public void LogError(string content)
		{
			Log($"[ERROR] {content}", ConsoleColor.Red);
		}

		/// <summary>
		/// Logs message as success.
		/// </summary>
		/// <param name="content">Content of the success.</param>
		public void LogSuccess(string content)
		{
			Log($"[SUCCESS] {content}", ConsoleColor.Green);
		}

		/// <summary>
		/// Opens a new log scope.
		/// </summary>
		public void OpenScope()
		{
			++_indentationLevel;
		}

		/// <summary>
		/// Closes the current log scope.
		/// </summary>
		public void CloseScope()
		{
			if (_indentationLevel == 0)
			{
				// level can't go below 0.
				return;
			}

			--_indentationLevel;
		}

		/// <summary>
		/// Generates the the prefix for a message.
		/// </summary>
		/// <returns>The prefix</returns>
		private string Prefix()
		{
			string prefix = "";
			for (int i = 0; i < _indentationLevel; ++i)
			{
				prefix += Indentation;
			}
			return prefix;
		}
	}

	/// <summary>
	/// Internal log wrapper class to use for core systems.
	/// </summary>
	internal static class Log
	{
		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <param name="content">Content of the message.</param>
		/// <param name="foreGround">Foreground color for the console.</param>
		/// <param name="backGround">Background color for the console.</param>
		public static void Write(string content, ConsoleColor foreGround = ConsoleColor.White, ConsoleColor backGround = ConsoleColor.Black)
		{
			LogService.Instance.Log(content, foreGround, backGround);
		}

		/// <summary>
		/// Logs message as info.
		/// </summary>
		/// <param name="content">Content of the info.</param>
		public static void Info(string content)
		{
			LogService.Instance.LogInfo(content);
		}

		/// <summary>
		/// Logs message as warning.
		/// </summary>
		/// <param name="content">Content of the warning.</param>
		public static void Warning(string content)
		{
			LogService.Instance.LogWarning(content);
		}

		/// <summary>
		/// Logs message as error.
		/// </summary>
		/// <param name="content">Content of the error.</param>
		public static void Error(string content)
		{
			LogService.Instance.LogError(content);
		}

		/// <summary>
		/// Logs message as success.
		/// </summary>
		/// <param name="content">Content of the success.</param>
		public static void Succes(string content)
		{
			LogService.Instance.LogSuccess(content);
		}

		/// <summary>
		/// Opens a new log scope.
		/// </summary>
		public static void OpenScope()
		{
			LogService.Instance.OpenScope();
		}

		/// <summary>
		/// Closes the current log scope.
		/// </summary>
		public static void CloseScope()
		{
			LogService.Instance.CloseScope();
		}
	}

	/// <summary>
	/// Utility class to control a log scope via using.
	/// </summary>
	public class LogScope : IDisposable
	{
		/// <summary>
		/// Opens the new scope.
		/// </summary>
		public LogScope()
		{
			LogService.Instance.OpenScope();
		}

		/// <summary>
		/// Closes the scope.
		/// </summary>
		public void Dispose()
		{
			LogService.Instance.CloseScope();
		}
	}
}