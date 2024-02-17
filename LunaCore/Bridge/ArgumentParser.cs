namespace Luna.Core
{
	/// <summary>
	/// Simple class to pass supported console arguments.
	/// </summary>
	public class ArgumentParser
	{
		public static ArgumentParser Instance { get; private set; } = new();

		/// <summary>
		/// No Compile flag for Luna Bridge.
		/// </summary>
		public bool NoCompile { get; private set; } = false;

		/// <summary>
		/// Path to luna config.
		/// </summary>
		public string ConfigPath { get; private set; } = "";

		/// <summary>
		/// Parses given console arguments.
		/// </summary>
		/// <param name="args">Console arguments</param>
		/// <returns>Returns true if parsing was successful.</returns>
		public bool Parse(string[] args)
		{
			LunaConsole.WriteLine("Parsing Arguments");

			List<string> listedArgs = new(args);

			if (listedArgs.Count == 0)
			{
				Help();

				return false;
			}

			LunaConsole.OpenScope();

			if (listedArgs.Contains("-nocode"))
			{
				if (listedArgs.Count > 1)
				{
					LunaConsole.WarningLine("No other argument besides -nocode is allowed.");
				}

				LunaConsole.WriteLine("Flag -nocode enabled. Skipping LunaBridge compile step.");

				NoCompile = true;

				LunaConsole.CloseScope();
				return true;
			}
			if (listedArgs.Contains("-config"))
			{
				int index = listedArgs.IndexOf("-config");
				if (index + 1 > listedArgs.Count - 1)
				{
					LunaConsole.ErrorLine("'-config' parameter is missing its value.");

					LunaConsole.CloseScope();
					return false;
				}

				ConfigPath = listedArgs[index + 1];
				if (!Path.Exists(ConfigPath))
				{
					LunaConsole.ErrorLine($"{ConfigPath} is not a valid path for '-config' parameter");

					LunaConsole.CloseScope();
					return false;
				}
			}
			if (listedArgs.Contains("--help"))
			{
				LunaConsole.CloseScope();

				Help();

				return false;
			}

			LunaConsole.CloseScope();
			return true;
		}

		/// <summary>
		/// Prints help.
		/// </summary>
		public void Help()
		{
			LunaConsole.WriteLine("Usage:");
			LunaConsole.WriteLine("\tLunaCLI.exe -code $PATH_TO_CODE$");
			LunaConsole.WriteLine("Help:");
			LunaConsole.WriteLine("\tLunaCLI.exe --help");
			LunaConsole.WriteLine("\t\tPrints help information for the Luna CLI.");
		}
	}
}