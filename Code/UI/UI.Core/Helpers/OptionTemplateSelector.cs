using Luna.Core;
using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;

namespace Luna.UI.Helpers;

/// <summary>
/// Selector class for the options tree view.
/// </summary>
public class OptionTemplateSelector : ITreeDataTemplate
{
	/// <summary>
	/// Gets the Templates for this selector.
	/// </summary>
	[Content]
	public Dictionary<string, IDataTemplate> Templates { get; } = [];

	/// <summary>
	/// Returns the instanced binding for this given object.
	/// </summary>
	/// <param name="item">Object that is used for the binding.</param>
	/// <returns>Instanced Binding for the option if successful, otherwise null.</returns>
	public InstancedBinding? ItemsSelector(object item)
	{
		if (item is IOption || item is OptionGroup)
		{
			return ((TreeDataTemplate)Templates[item.GetType().Name]).ItemsSelector(item);
		}
		else
			return null;
	}

	/// <summary>
	/// Checks if an object is valid for this selector.
	/// </summary>
	/// <param name="data">Object which should be checked.</param>
	/// <returns>True if it is valid, otherwise false.</returns>
	public bool Match(object? data)
	{
		return data is IOption || data is OptionGroup;
	}

	/// <summary>
	/// Builds the control.
	/// </summary>
	/// <param name="param">Parameter for this control construction.</param>
	/// <returns>Control instance if the valid object is requested, otherwise null.</returns>
	Control? ITemplate<object?, Control?>.Build(object? param)
	{
		return Templates[param?.GetType().Name ?? ""]?.Build(param);
	}
}