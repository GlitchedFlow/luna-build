﻿using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Generic interface that describes a build service.
	/// </summary>
	public interface IBuild
	{
		/// <summary>
		/// Registers the build services. Called by system.
		/// </summary>
		public void Register();

		/// <summary>
		/// Configurates the build service.
		/// </summary>
		public void Configurate();

		/// <summary>
		/// Generate the project.
		/// </summary>
		/// <param name="solution">The solution to which this project will be added.</param>
		/// <returns>Valid project if successful, otherwise false.</returns>
		public IProject? Generate(ISolution solution);
	}
}