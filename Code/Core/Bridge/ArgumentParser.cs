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
		/// Gets or sets the path to a script file.
		/// </summary>
		public string ScriptPath { get; set; } = "";

		/// <summary>
		/// File extension for luna scripts.
		/// </summary>
		public const string ScriptExtension = ".lusc";

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

			using LogScope scope = new();

			if (Path.Exists(listedArgs[0]) && File.Exists(listedArgs[0]) && Path.GetExtension(listedArgs[0]) == ScriptExtension)
			{
				ScriptPath = listedArgs[0];
			}

			if (listedArgs.Contains("-nocode"))
			{
				Log.Write("Flag -nocode enabled. Skipping LunaBridge compile step.");

				NoCompile = true;
			}
			if (listedArgs.Contains("-config"))
			{
				int index = listedArgs.IndexOf("-config");
				if (index + 1 > listedArgs.Count - 1)
				{
					Log.Error("'-config' parameter is missing its value.");

					return false;
				}

				ConfigPath = listedArgs[index + 1];
				if (!Path.Exists(ConfigPath))
				{
					Log.Error($"{ConfigPath} is not a valid path for '-config' parameter");

					return false;
				}
			}
			if (listedArgs.Contains("--help"))
			{
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