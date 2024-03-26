namespace Luna.Core
{
	/// <summary>
	/// Static class that provides functions related to the cache of luna.
	/// </summary>
	internal static class Cache
	{
		private const string c_cacheFolder = "lunaCache";

		/// <summary>
		/// Returns the path to the cache folder.
		/// </summary>
		/// <returns>The path to the cache folder.</returns>
		internal static string GetCacheFolder()
		{
			string path = Path.Combine(LunaConfig.Instance.SolutionPath, c_cacheFolder);
			if (!Path.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
	}
}