// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Avalonia.Controls;

namespace Luna.UI.Views;

/// <summary>
/// Main Window class.
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>
	/// Gets the Main window of the app instance.
	/// </summary>
	public static MainWindow? AppWindow { get; private set; } = null;

	/// <summary>
	/// Instanciates a new instance of the main window.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		AppWindow = this;
	}
}