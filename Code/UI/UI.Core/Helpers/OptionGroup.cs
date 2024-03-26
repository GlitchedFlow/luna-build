using Luna.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Luna.UI.Helpers;

/// <summary>
/// Helper class to make an option group to the UI.
/// </summary>
public class OptionGroup(string name) : ObservableObject
{
	private ObservableCollection<IOption> _children = [];

	/// <summary>
	/// Gets the name of the group.
	/// </summary>
	public string Name => name;

	/// <summary>
	/// Gets all options under this group.
	/// </summary>
	public ObservableCollection<IOption> Children => _children;
}