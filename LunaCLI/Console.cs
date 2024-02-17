using Luna.Core;
using System.Diagnostics.CodeAnalysis;

namespace Luna.CLI
{
	public class CConsole
	{
		[RequiresAssemblyFiles()]
		public static int Main(string[] args)
		{
			LunaConsole.WriteLine("Luna Build System");
			
			LunaConsole.OpenScope();

			LunaConsole.WriteLine("Initializing...");

			if (!ArgumentParser.Instance.Parse(args))
			{
				return -1;
			}

			if (!LunaConfig.Load(ArgumentParser.Instance.ConfigPath))
			{
				return -1;
			}

			if (!ArgumentParser.Instance.NoCompile)
			{
				Compiler compiler = new();
				if (!compiler.Compile(ArgumentParser.Instance.ConfigPath))
				{
					return -1;
				}
			}

			Kickstart kickstart = new();
			kickstart.Initialize();

			LunaConsole.CloseScope();

			return 0;
		}
	}
}