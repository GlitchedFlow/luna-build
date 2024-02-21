namespace Luna.Core
{
	/// <summary>
	/// Simple class to pass supported console arguments.
	/// </summary>
	public class ArgumentParser
	{
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
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
			Log.Write("Parsing Arguments");

			List<string> listedArgs = new(args);

			if (listedArgs.Count == 0)
			{
				Help();

				return false;
			}

			Log.OpenScope();

			if (listedArgs.Contains("-nocode"))
			{
				if (listedArgs.Count > 1)
				{
					Log.Warning("No other argument besides -nocode is allowed.");
				}

				Log.Write("Flag -nocode enabled. Skipping LunaBridge compile step.");

				NoCompile = true;

				Log.CloseScope();
				return true;
			}
			if (listedArgs.Contains("-config"))
			{
				int index = listedArgs.IndexOf("-config");
				if (index + 1 > listedArgs.Count - 1)
				{
					Log.Error("'-config' parameter is missing its value.");

					Log.CloseScope();
					return false;
				}

				ConfigPath = listedArgs[index + 1];
				if (!Path.Exists(ConfigPath))
				{
					Log.Error($"{ConfigPath} is not a valid path for '-config' parameter");

					Log.CloseScope();
					return false;
				}
			}
			if (listedArgs.Contains("--help"))
			{
				Log.CloseScope();

				Help();

				return false;
			}

			Log.CloseScope();
			return true;
		}

		/// <summary>
		/// Prints help.
		/// </summary>
		public void Help()
		{
			Log.Write("Usage:");
			Log.Write("\tLunaCLI.exe -config $PATH_TO_CONFIG$");
			Log.Write("Help:");
			Log.Write("\tLunaCLI.exe --help");
			Log.Write("\t\tPrints help information for the Luna CLI.");
		}
	}
}