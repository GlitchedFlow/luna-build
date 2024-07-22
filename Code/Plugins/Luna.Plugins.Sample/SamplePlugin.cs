// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

using Luna.Core;

namespace Luna.Plugins
{
	/// <summary>
	/// Meta service that can generate projects.
	/// </summary>
	public class SamplePlugin : IMeta
	{
		/// <summary>
		/// Registers the plugin with luna.
		/// </summary>
		public void Register()
		{
			ServiceProvider.RegistryService.RegisterMetaService(this);
		}

		// Add functionality in here which code from the luna bridge can use.
	}
}