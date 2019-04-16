using System;

namespace OnionSeed.Data
{
	/// <summary>
	/// Defines an object that is not fundamentally identified by its attributes, but rather by a unique identity value.
	/// </summary>
	/// <typeparam name="TIdentity">The type of the unique identity value.</typeparam>
	/// <remarks>Entities are not defined primarily by their attributes.
	/// They represent a thread of identity that runs through time and often across distinct representations.
	/// Sometimes an entity must be matched with another entity even though their attributes differ.
	/// Sometimes an entity must be distinguished from other entities even though their attributes are the same.
	/// Mistaken identity in these cases can lead to data corruption; thus, a unique identity value is created
	/// for the entity so it can be tracked regardless of the state of its attributes.</remarks>
	public interface IEntity<out TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Gets the unique identity value.
		/// </summary>
		TIdentity Id { get; }
	}
}
