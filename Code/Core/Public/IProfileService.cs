// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core
{
	/// <summary>
	/// Core service interface that is used to handle supported platforms (e.g. Debug, Profile, Release)
	/// </summary>
	public interface IProfileService : IMeta
	{
		/// <summary>
		/// Adds a new profile.
		/// </summary>
		/// <param name="profile">Name of the profile.</param>
		void AddProfile(string profile);

		/// <summary>
		/// Gets the count of registered profiles.
		/// </summary>
		/// <returns>Count of profiles</returns>
		int GetProfileCount();

		/// <summary>
		/// Gets the profile at the given index.
		/// </summary>
		/// <param name="index">Index of the profile.</param>
		/// <returns>Valid string if successful, otherwise Null.</returns>
		string? GetProfileAt(int index);

		/// <summary>
		/// Gets if the service has the given profile.
		/// </summary>
		/// <param name="profile">Profile</param>
		/// <returns>True if it has the profile, otherwise false.</returns>
		bool HasProfile(string profile);
	}
}