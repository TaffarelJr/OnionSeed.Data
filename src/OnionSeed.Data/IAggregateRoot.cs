using System;

namespace OnionSeed.Data
{
	/// <summary>
	/// Identifies the root entity of a collection of objects that are bound together (an aggregate).
	/// </summary>
	/// <typeparam name="TIdentity">The type of the unique identity value.</typeparam>
	/// <remarks>The aggregate root is a special type of <see cref="IEntity{TIdentity}"/> that
	/// guarantees the consistency of changes being made within the aggregate by forbidding
	/// external objects from holding references to its members.</remarks>
	public interface IAggregateRoot<out TIdentity> : IEntity<TIdentity>
		where TIdentity : IComparable<TIdentity>, IEquatable<TIdentity>
	{
	}
}
