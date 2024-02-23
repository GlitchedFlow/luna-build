using Luna.Core;
using Luna.Core.Target;
using System.Diagnostics.CodeAnalysis;

namespace Luna.CLI
{
	/// <summary>
	/// CLI host console.
	/// </summary>
	public class Console
	{
		/// <summary>
		/// Main entry point.
		/// </summary>
		/// <param name="args">consoel arguments</param>
		/// <returns>0 if no errors, otherwise -1.</returns>
		[RequiresAssemblyFiles()]
		public static int Main(string[] args)
		{
			Kickstart.InitializeCoreServices();

			ILogService? log = ServiceProvider.LogService;

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

			Kickstart.InitializeTargets();
			Kickstart.InitializePlugins();
			Kickstart.InitializeBridge();

			IConfiguratorService? configuratorService = ServiceProvider.ConfiguratorService;
			configuratorService?.Configurate();

			IGeneratorService? generatorService = ServiceProvider.GeneratorService;
			if (generatorService == null)
			{
				log?.LogError($"Generator Service is unavailable.");
				return -1;
			}

			int targetCount = ServiceProvider.RegistryService.GetTargetCount();
			if (targetCount <= 0)
			{
				log?.LogError($"No targets available.");
				return -1;
			}

			log?.Log($"Please enter the number of the target you want to generate the solution for. Available Targets ({targetCount}):");
			for (int curIndex = 0; curIndex < targetCount; ++curIndex)
			{
				ITarget? curTarget = ServiceProvider.RegistryService.GetTargetAt(curIndex);
				if (curTarget != null)
				{
					log?.Log($"{curIndex + 1}: {curTarget.Name}");
				}
			}

			bool validSelection = false;
			while (!validSelection)
			{
				if (int.TryParse(System.Console.ReadLine(), out int inputValue))
				{
					if (inputValue > targetCount || inputValue <= 0)
					{
						log?.LogError($"{inputValue} is out of range. Please enter a number between 1 and {targetCount}.");
					}
					else
					{
						generatorService.ActiveTarget = ServiceProvider.RegistryService.GetTargetAt(inputValue - 1);
						validSelection = true;
					}
				}
				else
				{
					log?.LogError($"Invalid selection. Please enter a number between 1 and {targetCount}.");
				}
			}

			log?.LogInfo($"Generating solution for: {generatorService?.ActiveTarget?.Name}");
			log?.LogInfo($"Solution Path: {Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), generatorService.ActiveTarget.SolutionFolder)}");

			bool? wasGenerated = generatorService?.Generate();

			if (wasGenerated != null && wasGenerated == false)
			{
				log?.LogError($"Solution was not generated.");
			}

			return 0;
		}
	}
}