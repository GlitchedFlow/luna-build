using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Luna.Core;
using Luna.UI.ViewModels;
using Luna.UI.Views;

namespace Luna.UI;

/// <summary>
/// Core app class.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Loads the app.axaml.
	/// </summary>
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	/// <summary>
	/// Initializes the UI.
	/// </summary>
	public override void OnFrameworkInitializationCompleted()
	{
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);

		MainViewModel viewModel = new();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			ArgumentParser.Instance.Parse(desktop.Args ?? []);

			if (!string.IsNullOrWhiteSpace(ArgumentParser.Instance.ScriptPath))
			{
				if (!CLI.Console.HandleScript(ArgumentParser.Instance.ScriptPath))
				{
					ServiceProvider.LogService?.LogError("Script did not execute successfully.");
				}
			}

			desktop.MainWindow = new MainWindow
			{
				DataContext = viewModel
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}