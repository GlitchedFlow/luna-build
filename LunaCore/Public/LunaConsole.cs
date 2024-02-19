namespace Luna.Core
{
	public class LunaConsole
	{
		private static LunaConsole m_instance = new();

		private LunaConsole()
		{
		}

		public string Indentation { get; set; } = "  ";
		public int Level { get; set; } = 0;

		public static LunaConsole Instance { get => m_instance; }

		public static void Write(string content, ConsoleColor foreGround = ConsoleColor.White, ConsoleColor backGround = ConsoleColor.Black)
		{
			Console.ForegroundColor = foreGround;
			Console.BackgroundColor = backGround;

			Console.Write($"{Prefix()}{content}");

			Console.ResetColor();
		}

		public static void WriteLine(string content, ConsoleColor foreGround = ConsoleColor.White, ConsoleColor backGround = ConsoleColor.Black)
		{
			Console.ForegroundColor = foreGround;
			Console.BackgroundColor = backGround;

			Console.WriteLine($"{Prefix()}{content}");

			Console.ResetColor();
		}

		public static void Info(string content)
		{
			Write(content, ConsoleColor.Blue);
		}

		public static void InfoLine(string content)
		{
			WriteLine(content, ConsoleColor.Blue);
		}

		public static void Warning(string content)
		{
			Write(content, ConsoleColor.Yellow);
		}

		public static void WarningLine(string content)
		{
			WriteLine(content, ConsoleColor.Yellow);
		}

		public static void Error(string content)
		{
			Write(content, ConsoleColor.Red);
		}

		public static void ErrorLine(string content)
		{
			WriteLine(content, ConsoleColor.Red);
		}

		private static string Prefix()
		{
			string prefix = "";
			for (int i = 0; i < m_instance.Level; ++i)
			{
				prefix += m_instance.Indentation;
			}
			return prefix;
		}

		public static void OpenScope()
		{
			++m_instance.Level;
		}

		public static void CloseScope()
		{
			--m_instance.Level;
		}
	}
}