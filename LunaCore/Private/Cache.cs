namespace Luna.Core
{
	internal static class Cache
	{
		private const string c_cacheFolder = "lunaCache";

		internal static string GetCacheFolder()
		{
			return Path.Combine(LunaConfig.Instance.SolutionPath, c_cacheFolder);
		}
	}
}
