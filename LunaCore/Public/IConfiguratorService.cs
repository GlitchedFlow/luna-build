namespace Luna.Core
{
	/// <summary>
	/// Core service interface that is used to configurate build services.
	/// </summary>
	public interface IConfiguratorService : IMeta
	{
		/// <summary>
		/// Configurate all build services.
		/// </summary>
		void Configurate();
	}
}