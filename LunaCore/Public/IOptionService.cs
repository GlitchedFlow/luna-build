namespace Luna.Core
{
	public interface IOption : IIdentifier
	{
		string Name { get; }
		string Description { get; set; }
		bool IsEnabled { get; set; }
		string Category { get; set; }
	}

	public interface IOptionService : IMeta
	{
		IOption? RegisterOption(Guid guid, string name, bool IsEnabled);
		bool? IsOptionEnabled(Guid guid);
		bool? IsOptionEnabled(string name);
		bool RemoveOption(Guid guid);
	}
}
