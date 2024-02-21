namespace Luna.Core
{
	/// <summary>
	/// Generic interface to describe an object that is identifiable by its guid.
	/// </summary>
	public interface IIdentifier
	{
		/// <summary>
		/// Gets the guid of this object.
		/// </summary>
		public Guid Guid { get; }
	}
}