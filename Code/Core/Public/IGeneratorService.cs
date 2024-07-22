// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core.Target;

namespace Luna.Core
{
	/// <summary>
	/// Core service interface that is used to generate all build services.
	/// </summary>
	public interface IGeneratorService : IMeta
	{
		/// <summary>
		/// Gets or sets the active target.
		/// </summary>
		ITarget? ActiveTarget { get; set; }

		/// <summary>
		/// Triggers the solution generation.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		bool Generate();
	}
}