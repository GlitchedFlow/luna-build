// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Avalonia.Platform.Storage;
using Luna.CLI;
using Luna.Core;
using Luna.Core.Target;
using Luna.UI.Views;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Luna.UI.Helpers;
using Avalonia.Threading;

namespace Luna.UI.ViewModels;

/// <summary>
/// Main view model for the UI.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
	private string _input = "";
	private StringWriter _consoleWriter = new();
	private RelayCommand _searchCodePathCommand;
	private RelayCommand _searchMetaPathCommand;
	private RelayCommand _searchSolutionPathCommand;
	private RelayCommand _searchWorkspacePathCommand;
	private RelayCommand<string> _generateCommand;
	private AsyncRelayCommand _configurateCommand;
	private AsyncRelayCommand _loadCommand;
	private ObservableCollection<Entry> _plugins = [];
	private ObservableCollection<Entry> _targets = [];
	private ObservableCollection<OptionGroup> _optionGroups = [];
	private bool _isBusy = false;

	/// <summary>
	/// Gets the file type picker for luna configs.
	/// </summary>
	private static FilePickerFileType _lunaFileType { get; } = new("Luna Config")
	{
		Patterns = ["*.luna"]
	};

	/// <summary>
	/// Instanciates a new instance of the main view model.
	/// </summary>
	public MainViewModel()
	{
		System.Console.SetOut(_consoleWriter);

		Kickstart.InitializeCoreServices();

		ILogService? log = ServiceProvider.LogService;

		BaseModule.InitModules();

		log?.Log($"Please enter a command that should be executed.");
		log?.Log($"Enter \"help\" to get help for the current module.");
		log?.Log($"Enter \"exit\" to exit the current module.");

		_searchCodePathCommand = new RelayCommand(SearchCodePath);
		_searchMetaPathCommand = new RelayCommand(SearchMetaPath);
		_searchSolutionPathCommand = new RelayCommand(SearchSolutionPath);
		_searchWorkspacePathCommand = new RelayCommand(SearchWorkspacePath);
		_generateCommand = new RelayCommand<string>(Generate, CanGenerate);
		_configurateCommand = new AsyncRelayCommand(Configurate, CanConfigurate);
		_loadCommand = new AsyncRelayCommand(Load);

		UpdatePluginsAndTargets();
	}

	/// <summary>
	/// Destructor to release the console writer.
	/// </summary>
	~MainViewModel()
	{
		_consoleWriter.Dispose();
	}

	/// <summary>
	/// Gets the output from the console.
	/// </summary>
	public string Output => _consoleWriter.ToString();

	/// <summary>
	/// Gets or sets the input for the CLI input field.
	/// </summary>
	public string Input
	{
		get => _input;
		set
		{
			_input = value;
			if (_input.Contains("\r\n"))
			{
				if (BaseModule.ActiveModule?.Name == "Core" && _input.ToLower() == "exit\r\n")
				{
					ILogService? log = ServiceProvider.LogService;
					log?.LogError("Can't leave core module in GUI mode");
				}
				else
				{
					_consoleWriter.Write($"[{BaseModule.ActiveModule?.Name}] {_input}");
					Execute(_input);
				}

				_input = "";
				OnPropertyChanged();
				OnPropertyChanged(nameof(Output));
			}
		}
	}

	/// <summary>
	/// Gets or sets the path to the code files.
	/// </summary>
	public string CodePath
	{
		get => LunaConfig.Instance.CodePath;
		set
		{
			LunaConfig.Instance.CodePath = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Gets or sets the path to the meta files.
	/// </summary>
	public string MetaPath
	{
		get => LunaConfig.Instance.MetaPath;
		set
		{
			LunaConfig.Instance.MetaPath = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Gets or sets the solution path.
	/// </summary>
	public string SolutionPath
	{
		get => LunaConfig.Instance.SolutionPath;
		set
		{
			LunaConfig.Instance.SolutionPath = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Gets or sets the workspace path.
	/// </summary>
	public string WorkspacePath
	{
		get => LunaConfig.Instance.WorkspacePath;
		set
		{
			LunaConfig.Instance.WorkspacePath = value;
			OnPropertyChanged();

			UpdatePluginsAndTargets();
		}
	}

	/// <summary>
	/// Gets or sets the main name of the solution.
	/// </summary>
	public string Name
	{
		get => LunaConfig.Instance.Name;
		set
		{
			LunaConfig.Instance.Name = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Gets a list of all available targets.
	/// </summary>
	public List<string> AvailableTargets
	{
		get
		{
			int count = ServiceProvider.RegistryService.GetTargetCount();
			List<string> result = [];
			for (int curIndex = 0; curIndex < count; ++curIndex)
			{
				ITarget? target = ServiceProvider.RegistryService.GetTargetAt(curIndex);
				if (target != null)
				{
					result.Add(target.Name);
				}
			}
			return result;
		}
	}

	/// <summary>
	/// Gets a collection of all option groups.
	/// </summary>
	public ObservableCollection<OptionGroup> OptionGroups => _optionGroups;

	/// <summary>
	/// Gets a collection of all available plugin libraries.
	/// </summary>
	public ObservableCollection<Entry> Plugins => _plugins;

	/// <summary>
	/// Gets a collection of all available target libraries.
	/// </summary>
	public ObservableCollection<Entry> Targets => _targets;

	/// <summary>
	/// Gets the command to search for the code directory.
	/// </summary>
	public RelayCommand SearchCodePathCommand => _searchCodePathCommand;

	/// <summary>
	/// Gets the command to search for the meta directory.
	/// </summary>
	public RelayCommand SearchMetaPathCommand => _searchMetaPathCommand;

	/// <summary>
	/// Gets the command to search for the solution directory.
	/// </summary>
	public RelayCommand SearchSolutionPathCommand => _searchSolutionPathCommand;

	/// <summary>
	/// Gets the command to search for the workspace directory.
	/// </summary>
	public RelayCommand SearchWorkspacePathCommand => _searchWorkspacePathCommand;

	/// <summary>
	/// Gets the command to generate the solution.
	/// </summary>
	public RelayCommand<string> GenerateCommand => _generateCommand;

	/// <summary>
	/// Gets the command to configurate Luna.
	/// </summary>
	public AsyncRelayCommand ConfigurateCommand => _configurateCommand;

	/// <summary>
	/// Gets the command to load a luna config.
	/// </summary>
	public AsyncRelayCommand LoadCommand => _loadCommand;

	/// <summary>
	/// Gets if luna was already configurated.
	/// </summary>
	public bool IsConfigurated => CanGenerate(null);

	/// <summary>
	/// Gets or sets if the UI is busy with something.
	/// </summary>
	public bool IsBusy
	{
		get => _isBusy;
		set
		{
			_isBusy = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Search for the code path.
	/// </summary>
	private async void SearchCodePath()
	{
		await PickPath(CodePath, "Select Code Path", nameof(CodePath));
	}

	/// <summary>
	/// Search for the meta path.
	/// </summary>
	private async void SearchMetaPath()
	{
		await PickPath(MetaPath, "Select Meta Path", nameof(MetaPath));
	}

	/// <summary>
	/// Search for the solution path.
	/// </summary>
	private async void SearchSolutionPath()
	{
		await PickPath(SolutionPath, "Select Solution Path", nameof(SolutionPath));
	}

	/// <summary>
	/// Search for the workspace path.
	/// </summary>
	private async void SearchWorkspacePath()
	{
		await PickPath(WorkspacePath, "Select Workspace Path", nameof(WorkspacePath));
	}

	/// <summary>
	/// Search for a path and assign it to a property in the config.
	/// </summary>
	/// <param name="startLocation">Start location for the search.</param>
	/// <param name="title">Title of the search dialog.</param>
	/// <param name="property">Name of the property to which the path should be assign.</param>
	/// <returns>Task</returns>
	private async Task PickPath(string startLocation, string title, string property)
	{
		if (MainWindow.AppWindow == null)
		{
			return;
		}

		var result = await MainWindow.AppWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
		{
			AllowMultiple = false,
			SuggestedStartLocation = await MainWindow.AppWindow.StorageProvider.TryGetFolderFromPathAsync(startLocation),
			Title = title
		});

		if (result != null && result.Count > 0)
		{
			Execute($"set {property.ToLower()} {result[0].TryGetLocalPath()}");
			OnPropertyChanged(property);
		}
	}

	/// <summary>
	/// Executes the input on the CLI.
	/// </summary>
	/// <param name="input">Input to execute on the CLI.</param>
	/// <returns>True if successful, otherwise false.</returns>
	private bool Execute(string input)
	{
		if (!CLI.Console.Execute(input))
		{
			return false;
		}

		Dispatcher.UIThread.Post(() =>
		{
			OnPropertyChanged(nameof(Output));
			OnPropertyChanged(nameof(CodePath));
			OnPropertyChanged(nameof(MetaPath));
			OnPropertyChanged(nameof(SolutionPath));
			OnPropertyChanged(nameof(WorkspacePath));
			OnPropertyChanged(nameof(AvailableTargets));
			OnPropertyChanged(nameof(Name));

			UpdatePluginsAndTargets();
			UpdateOptions();

			ConfigurateCommand.NotifyCanExecuteChanged();
			GenerateCommand.NotifyCanExecuteChanged();

			OnPropertyChanged(nameof(IsConfigurated));
		});

		return true;
	}

	/// <summary>
	/// Check if the solution can be generated.
	/// </summary>
	/// <param name="target">Possible target.</param>
	/// <returns>True if yes, otherwise false.</returns>
	private bool CanGenerate(object? target)
	{
		return CanConfigurate() && ServiceProvider.RegistryService.GetTargetCount() > 0;
	}

	/// <summary>
	/// Sets an active target and generates the solution.
	/// </summary>
	/// <param name="target">Target which should be set as active.</param>
	private void Generate(object? target)
	{
		Execute($"targets.set \"{(string.IsNullOrWhiteSpace((string?)target) ? AvailableTargets[0] : (string?)target)}\"");
		Execute("generator.generate");
	}

	/// <summary>
	/// Checks if Luna can be configurated.
	/// </summary>
	/// <returns>True if yes, otherwise false.</returns>
	private bool CanConfigurate()
	{
		return LunaConfig.Instance;
	}

	/// <summary>
	/// Configurates Luna.
	/// </summary>
	/// <returns>Task</returns>
	private async Task Configurate()
	{
		IsBusy = true;
		await Task.Run(() =>
		{
			Execute("compile");
			Execute("init targets");
			Execute("init plugins");
			Execute("init bridge");
		});
		IsBusy = false;
	}

	/// <summary>
	/// Searches and loads a luna config.
	/// </summary>
	/// <returns>Task</returns>
	private async Task Load()
	{
		if (MainWindow.AppWindow == null)
		{
			return;
		}

		var result = await MainWindow.AppWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
		{
			AllowMultiple = false,
			Title = "Load config",
			FileTypeFilter = [_lunaFileType]
		});

		if (result != null && result.Count > 0)
		{
			Execute($"load {result[0].TryGetLocalPath()}");
		}
	}

	/// <summary>
	/// Updates available plugin and target libraries based on the current workspace directory.
	/// </summary>
	private void UpdatePluginsAndTargets()
	{
		ObservableCollection<Entry> _pluginsCopy = new(_plugins);
		_plugins.Clear();

		ObservableCollection<Entry> _targetsCopy = new(_targets);
		_targets.Clear();

		DirectoryInfo curPluginDir = new(Path.Combine(LunaConfig.Instance.WorkspacePath, Kickstart.PluginsDir));
		ScanDir(curPluginDir, _plugins);

		curPluginDir = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Kickstart.PluginsDir));

		ScanDir(curPluginDir, _plugins);

		RecoverState(_plugins, _pluginsCopy);

		OnPropertyChanged(nameof(Plugins));

		DirectoryInfo curTargetDir = new(Path.Combine(LunaConfig.Instance.WorkspacePath, Kickstart.TargetsDir));
		ScanDir(curTargetDir, _targets);

		curPluginDir = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Kickstart.TargetsDir));

		ScanDir(curPluginDir, _targets);

		RecoverState(_targets, _targetsCopy);

		OnPropertyChanged(nameof(Targets));
	}

	/// <summary>
	/// Scans a directory for all *.dll files and adds them as possible entry.
	/// </summary>
	/// <param name="dir">Directory info</param>
	/// <param name="entries">Collection of entries which should be filled.</param>
	private static void ScanDir(DirectoryInfo dir, ObservableCollection<Entry> entries)
	{
		if (!Directory.Exists(dir.FullName))
		{
			return;
		}

		foreach (FileInfo file in dir.GetFiles("*.dll"))
		{
			string fileName = Path.GetFileNameWithoutExtension(file.Name);
			if (entries.Count(x => x.Name == fileName) == 0)
			{
				entries.Add(new(fileName, true));
			}
		}
	}

	/// <summary>
	/// Syncs the state of matching entries.
	/// </summary>
	/// <param name="newOne">Collection with new entries.</param>
	/// <param name="oldOne">Collection with old entries.</param>
	private static void RecoverState(ObservableCollection<Entry> newOne, ObservableCollection<Entry> oldOne)
	{
		newOne.ToList().ForEach(newEntry =>
		{
			Entry? matchingEntry = oldOne.FirstOrDefault(oldEntry => oldEntry.Name == newEntry.Name);
			if (matchingEntry != null)
			{
				newEntry.IsEnabled = matchingEntry.IsEnabled;
			}
		});
	}

	/// <summary>
	/// Gets all options from the option service and exposes them through OptionGroups.
	/// </summary>
	private void UpdateOptions()
	{
		IOptionService? optionService = ServiceProvider.OptionService;

		OptionGroups.Clear();

		optionService?.VisitGroupedOptions(x =>
		{
			OptionGroup newGroup = new OptionGroup(x.Key == null ? "Uncategorized" : x.Key);
			foreach (IOption item in x)
			{
				newGroup.Children.Add(item);
			}
			OptionGroups.Add(newGroup);
			return true;
		});
	}
}