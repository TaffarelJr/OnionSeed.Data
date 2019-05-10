using System;
using System.Collections.Generic;

namespace OnionSeed.Data
{
	/// <summary>
	/// Defines a mechanism that can be used to query entities from a data store.
	/// </summary>
	/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	public interface IQueryService<TRoot, in TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Gets the number of entities in the data store.
		/// </summary>
		/// <returns>The number of entities in the data store.</returns>
		long GetCount();

		/// <summary>
		/// Begins an enumeration of all entities in the data store.
		/// </summary>
		/// <returns>An enumeration of all entities in the data store.</returns>
		IEnumerable<TRoot> GetAll();

		/// <summary>
		/// Gets a specific entity from the data store by its unique identity value.
		/// </summary>
		/// <param name="id">The unique identity value of the entity.</param>
		/// <returns>The entity that has the given unique identity value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		/// <exception cref="KeyNotFoundException">The specified unique identity value does not exist in the data store.</exception>
		TRoot GetById(TIdentity id);

		/// <summary>
		/// Attempts to get a specific entity from the data store by its unique identity value.
		/// </summary>
		/// <param name="id">The unique identity value of the entity.</param>
		/// <param name="result">When this method returns, contains the entity that has the given unique identity value,
		/// or <c>null</c> if the specified entity was not found. This parameter is passed uninitialized;
		/// any value originally supplied in <paramref name="result"/> will be overwritten.</param>
		/// <returns><c>true</c> if the specified entity was found; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		bool TryGetById(TIdentity id, out TRoot result);
	}
}
