namespace Luna.Core
{
	/// <summary>
	/// Extension class for strings.
	/// </summary>
	public static class LunaGuid
	{
		/// <summary>
		/// Parses the string to a guid.
		/// </summary>
		/// <param name="guidString">The guid as a string.</param>
		/// <returns>The guid</returns>
		public static Guid AsGuid(this string guidString)
		{
			return new Guid(guidString);
		}
	}
}