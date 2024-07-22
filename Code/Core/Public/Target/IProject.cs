// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core.Target
{
	/// <summary>
	/// Generic interface that describes a project.
	/// </summary>
	public interface IProject : IIdentifier
	{
		/// <summary>
		/// Writes the project file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool WriteFile();
	}
}