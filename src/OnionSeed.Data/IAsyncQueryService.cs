using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data
{
	/// <summary>
	/// Defines a mechanism that can be used to asynchronously query entities from a data store.
	/// </summary>
	/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	public interface IAsyncQueryService<TRoot, in TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Gets the number of entities in the data store.
		/// </summary>
		/// <returns>A task representing the operation. Upon completion, it will contain
		/// the number of entities in the data store.</returns>
		Task<long> GetCountAsync();

		/// <summary>
		/// Begins an enumeration of all entities in the data store.
		/// </summary>
		/// <returns>A task representing the operation. Upon completion, it will contain
		/// an enumeration of all entities in the data store.</returns>
		Task<IEnumerable<TRoot>> GetAllAsync();

		/// <summary>
		/// Gets a specific entity from the data store by its unique identity value.
		/// </summary>
		/// <param name="id">The unique identity value of the entity.</param>
		/// <returns>A task representing the operation. Upon completion, it will contain
		/// the entity that has the given unique identity value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		/// <exception cref="KeyNotFoundException">The specified unique identity value does not exist in the data store.</exception>
		Task<TRoot> GetByIdAsync(TIdentity id);

		/// <summary>
		/// Attempts to get a specific entity from the data store by its unique identity value.
		/// </summary>
		/// <param name="id">The unique identity value of the entity.</param>
		/// <returns>A task representing the operation. Upon completion, it will contain
		/// the entity that has the given unique identity value,
		/// or <c>null</c> if the specified entity was not found.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		Task<TRoot> TryGetByIdAsync(TIdentity id);
	}
}
