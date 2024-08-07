// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core
{
	/// <summary>
	/// Generic interface that describes a meta service.
	/// </summary>
	public interface IMeta : ILuna
	{
		/// <summary>
		/// Registers the meta service. Called by system.
		/// </summary>
		void Register();
	}
}