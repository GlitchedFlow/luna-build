namespace Luna.Core
{
	/// <summary>
	/// Core service class that is used to handle profiles during solution generation.
	/// </summary>
	internal class ProfileService : IProfileService
	{
		private readonly List<string> _profiles = [];

		/// <summary>
		/// Adds a profile.
		/// </summary>
		/// <param name="profile">Name of the profile.</param>
		public void AddProfile(string profile)
		{
			if (_profiles.FirstOrDefault(p => p == profile) != null)
			{
				// Profile already added.
				return;
			}

			_profiles.Add(profile);
		}

		/// <summary>
		/// Gets the count of registered profiles.
		/// </summary>
		/// <returns>Count of profiles</returns>
		public int GetProfileCount()
		{
			return _profiles.Count;
		}

		/// <summary>
		/// Gets the profile at the given index.
		/// </summary>
		/// <param name="index">Index of the profile.</param>
		/// <returns>Valid string if successful, otherwise Null.</returns>
		public string? GetProfileAt(int index)
		{
			if (index >= GetProfileCount())
			{
				return null;
			}

			return _profiles[index];
		}

		/// <summary>
		/// Gets if the service has the given profile.
		/// </summary>
		/// <param name="profile">Profile</param>
		/// <returns>True if it has the profile, otherwise false.</returns>
		public bool HasProfile(string profile)
		{
			return _profiles.FirstOrDefault(p => p == profile) != null;
		}

		/// <summary>
		/// Registers the service. Called by system.
		/// </summary>
		public void Register()
		{
			RegistryService.Instance.RegisterMetaService((IProfileService)this);
		}
	}
}