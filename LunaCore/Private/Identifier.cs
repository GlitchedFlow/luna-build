using System.Text.Json.Serialization;

namespace Luna.Core
{
	internal class Identifier(Guid guid) : IIdentifier
	{
		public Guid Guid { get; init; } = guid;
	}

	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(Identifier))]
	internal partial class IdentifierSourceGenerationContext : JsonSerializerContext
	{
	}
}