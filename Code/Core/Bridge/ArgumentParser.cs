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
				return true;
			}

			using LogScope scope = new();

			if (!File.Exists(listedArgs[0]) || Path.GetExtension(listedArgs[0]) != ScriptExtension)
			{
				Log.Error($"{listedArgs[0]} is not a path to valid luna script file.");
				return false;
			}

			ScriptPath = listedArgs[0];

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
			Log.Write("\tLunaCLI.exe $PATH_TO_SCRIPTFILE$");
			Log.Write("Help:");
			Log.Write("\tLunaCLI.exe --help");
			Log.Write("\t\tPrints help information for the Luna CLI.");
		}
	}
}