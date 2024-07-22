// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Luna.UI.Helpers;

/// <summary>
/// Small Helper class to map a togglable plugin or target to the UI.
/// </summary>
public class Entry(string name, bool isEnabled) : ObservableObject
{
	private bool _isEnabled = isEnabled;

	/// <summary>
	/// Gets the name of the entry.
	/// </summary>
	public string Name => name;

	/// <summary>
	/// Gets or sets if this entry is enabled.
	/// </summary>
	public bool IsEnabled
	{
		get => _isEnabled;
		set
		{
			_isEnabled = value;
			OnPropertyChanged(nameof(IsEnabled));
		}
	}
}