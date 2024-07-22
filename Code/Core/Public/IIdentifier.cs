// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core
{
	/// <summary>
	/// Generic interface to describe an object that is identifiable by its guid.
	/// </summary>
	public interface IIdentifier
	{
		/// <summary>
		/// Gets the guid of this object.
		/// </summary>
		Guid Guid { get; }
	}
}