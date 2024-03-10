namespace Luna.Core
{
	/// <summary>
	/// Generic interface that describes a meta service.
	/// </summary>
	public interface IMeta
	{
		/// <summary>
		/// Registers the meta service. Called by system.
		/// </summary>
		void Register();
	}
}