// Copyright 2024 - Florian Hoeschel
// Licensed to you under MIT license.

namespace Luna.Core
{
	/// <summary>
	/// Base abstract class to give something an identifier.
	/// </summary>
	/// <param name="guid">Guid to identifier this instance by.</param>
	internal abstract class Identifier(Guid guid) : IIdentifier
	{
		/// <summary>
		/// Gets the guid of this instance.
		/// </summary>
		public Guid Guid { get; init; } = guid;
	}
}