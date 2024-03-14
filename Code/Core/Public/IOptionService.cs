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
		/// Gets or sets the category of this option.
		/// </summary>
		string Category { get; set; }

		/// <summary>
		/// Gets or sets the option guid which this option depends on.
		/// </summary>
		Guid DependsOn { get; set; }
	}

	/// <summary>
	/// Generic interface that describes a value option to show readonly const values.
	/// </summary>
	public interface IValueOption : IOption
	{
		/// <summary>
		/// Gets the value of an option.
		/// </summary>
		string Value { get; }
	}

	/// <summary>
	/// Generic interface that describes a flag option to enable/disable section of the solution.
	/// </summary>
	public interface IFlagOption : IOption
	{
		/// <summary>
		/// Gets or sets if this option is enabled.
		/// </summary>
		bool IsEnabled { get; set; }
	}

	/// <summary>
	/// Core service interface that is used to handle options for the generation.
	/// </summary>
	public interface IOptionService : IMeta
	{
		/// <summary>
		/// Registers a new flag option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="IsEnabled">Is this option enabled?</param>
		/// <param name="category">Category of the option.</param>
		/// <param name="dependsOn">The guid of the option this option depends on.</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		IFlagOption? RegisterFlagOption(Guid guid, string name, bool IsEnabled, string? category = null, Guid? dependsOn = null);

		/// <summary>
		/// Registers a new option.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <param name="name">The name of the option.</param>
		/// <param name="value">Const value of something.</param>
		/// <param name="category">Category of the option.</param>
		/// <param name="dependsOn">The guid of the option this option depends on.</param>
		/// <returns>The new option. Null if unsuccessful.</returns>
		IValueOption? RegisterValueOption(Guid guid, string name, string value, string? category = null, Guid? dependsOn = null);

		/// <summary>
		/// Checks if an option is enabled.
		/// </summary>
		/// <param name="guid">The guid of the option.</param>
		/// <returns>Returns true if enabled, otherwise false. Null if option does not exist.</returns>
		bool? IsOptionEnabled(Guid guid);

		/// <summary>
		/// Builds the dependency tree between the options.
		/// </summary>
		void BuildDependencyTree();

		/// <summary>
		/// Visit all options.
		/// </summary>
		/// <param name="visitor">Visitor function</param>
		void VisitOptions(Func<IOption, bool> visitor);

		/// <summary>
		/// Visit all options grouped by their category.
		/// </summary>
		/// <param name="visitor">Visitor function</param>
		void VisitGroupedOptions(Func<IGrouping<string, IOption>, bool> visitor);
	}
}