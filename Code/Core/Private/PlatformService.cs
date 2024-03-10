namespace Luna.Core
{
	/// <summary>
	/// Core service class that is used to handle platforms during solution generation.
	/// </summary>
	internal class PlatformService : IPlatformService
	{
		private readonly List<string> _platforms = [];

		/// <summary>
		/// Adds a new platform.
		/// </summary>
		/// <param name="platform">Name of the platform.</param>
		public void AddPlatform(string platform)
		{
			_platforms.Add(platform);
		}

		/// <summary>
		/// Gets the count of registered platforms.
		/// </summary>
		/// <returns>Count of platforms</returns>
		public int GetPlatformCount()
		{
			return _platforms.Count;
		}

		/// <summary>
		/// Gets the platforms at the given index.
		/// </summary>
		/// <param name="index">Index of the platform.</param>
		/// <returns>Valid string if successful, otherwise Null.</returns>
		public string? GetPlatformAt(int index)
		{
			if (index >= GetPlatformCount())
			{
				return null;
			}
			return _platforms[index];
		}

		/// <summary>
		/// Registers the service. Called by system.
		/// </summary>
		public void Register()
		{
			RegistryService.Instance.RegisterMetaService((IPlatformService)this);
		}
	}
}