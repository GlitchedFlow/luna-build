// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core
{
	/// <summary>
	/// Core service interface that is used to handle supported platforms (e.g. x64, x86, Any CPU)
	/// </summary>
	public interface IPlatformService : IMeta
	{
		/// <summary>
		/// Adds a new platform.
		/// </summary>
		/// <param name="platform">Platform name</param>
		void AddPlatform(string platform);

		/// <summary>
		/// Gets the count of registered platforms.
		/// </summary>
		/// <returns>Count of platforms</returns>
		int GetPlatformCount();

		/// <summary>
		/// Gets the platforms at the given index.
		/// </summary>
		/// <param name="index">Index of the platform.</param>
		/// <returns>Valid string if successful, otherwise Null.</returns>
		string? GetPlatformAt(int index);

		/// <summary>
		/// Gets if the service has the given platform.
		/// </summary>
		/// <param name="platform">Platform</param>
		/// <returns>True if it has the platform, otherwise false.</returns>
		bool HasPlatform(string platform);
	}
}