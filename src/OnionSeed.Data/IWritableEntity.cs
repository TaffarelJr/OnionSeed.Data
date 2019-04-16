using System;

namespace OnionSeed.Types
{
	/// <inheritdoc />
	/// <summary>
	/// Represents an entity in a state where it's unique identity value can be set
	/// (for example, when creating a new instance or re-hydrating an instance from a data store).
	/// </summary>
	public interface IWritableEntity<TIdentity> : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Gets or sets the unique identity value.
		/// </summary>
		new TIdentity Id { get; set; }
	}
}
