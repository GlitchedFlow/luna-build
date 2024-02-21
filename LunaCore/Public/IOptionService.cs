namespace Luna.Core
{
	/// <summary>
	/// Generic interface that describes an option.
	/// </summary>
	public interface IOption : IIdentifier
	{
		/// <summary>
		/// Gets the name of the option.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets or sets the description of the option.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets or sets if this option is enabled.
		/// </summary>
		bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the category of this option.
		/// </summary>
		string Category { get; set; }

		/// <summary>
		/// Gets or sets the option guid which this option depends on.
		/// </summary>
		Guid DependsOn { get; set; }
	}

	/// <summary>
	/// Core service interface that is used to handle options for the generation.
	/// </summary>
	public interface IOptionService : IMeta
	{
		/// <summary>
		/// Registers a new option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="IsEnabled">Is this option enabled?</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		IOption? RegisterOption(Guid guid, string name, bool IsEnabled);

		/// <summary>
		/// Checks if an option is enabled.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <returns>Returns true if enabled, otherwise false. Null if option does not exist.</returns>
		bool? IsOptionEnabled(Guid guid);

		/// <summary>
		/// Checks if an option is enabled.
		/// </summary>
		/// <param name="name">The name of the option.</param>
		/// <returns>Returns true if enabled, otherwise false. Null if option does not exist.</returns>
		bool? IsOptionEnabled(string name);

		/// <summary>
		/// Saves the current state of options to file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool SaveToFile();

		/// <summary>
		/// Loads options state from file and tries to apply the state of each if available.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool LoadFromFile();
	}
}