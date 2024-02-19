using Luna.Core;
using Luna.Core.Target;
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
				if (!Compiler.Compile(ArgumentParser.Instance.ConfigPath))
				{
					return -1;
				}
			}

			Kickstart.Initialize();

			IConfiguratorService? configuratorService = ServiceProvider.RegistryService.GetMetaService<IConfiguratorService>();
			configuratorService?.Configurate();

			IGeneratorService? generatorService = ServiceProvider.RegistryService.GetMetaService<IGeneratorService>();
			if (generatorService == null)
			{
				LunaConsole.ErrorLine($"Generator Service is unavailable.");
				return -1;
			}

			int targetCount = ServiceProvider.RegistryService.GetTargetCount();
			if (targetCount <= 0)
			{
				LunaConsole.ErrorLine($"No targets available.");
				return -1;
			}

			LunaConsole.WriteLine($"Please enter the number of the target you want to generate the solution for. Available Targets ({targetCount}):");
			for (int curIndex = 0; curIndex < targetCount; ++curIndex)
			{
				ITarget? curTarget = ServiceProvider.RegistryService.GetTargetAt(curIndex);
				if (curTarget != null)
				{
					LunaConsole.WriteLine($"{curIndex + 1}: {curTarget.Name}");
				}
			}

			bool validSelection = false;
			while (!validSelection)
			{
				if (int.TryParse(Console.ReadLine(), out int inputValue))
				{
					if (inputValue > targetCount || inputValue <= 0)
					{
						LunaConsole.ErrorLine($"{inputValue} is out of range. Please enter a number between 1 and {targetCount}.");
					}
					else
					{
						generatorService.ActiveTarget = ServiceProvider.RegistryService.GetTargetAt(inputValue - 1);
						validSelection = true;
					}
				}
				else
				{
					LunaConsole.ErrorLine($"Invalid selection. Please enter a number between 1 and {targetCount}.");
				}
			}

			LunaConsole.InfoLine($"Generating solution for: {generatorService?.ActiveTarget?.Name}");
			LunaConsole.InfoLine($"Solution Path: {Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), generatorService.ActiveTarget.SolutionFolder)}");

			bool? wasGenerated = generatorService?.Generate();

			if (wasGenerated != null && wasGenerated == false)
			{
			}

			LunaConsole.CloseScope();

			return 0;
		}
	}
}