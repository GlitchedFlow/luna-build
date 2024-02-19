namespace Luna.Core
{
	public static class LunaGuid
	{
		public static Guid AsGuid(this string guidString)
		{
			return new Guid(guidString);
		}
	}
}