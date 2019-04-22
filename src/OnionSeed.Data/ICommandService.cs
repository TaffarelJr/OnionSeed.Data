using System;
using System.Collections.Generic;

namespace OnionSeed.Data
{
	/// <summary>
	/// Defines a mechanism that can be used to update entities in a data store.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	public interface ICommandService<TEntity, in TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Adds an entity to the data store.
		/// </summary>
		/// <param name="item">The entity to be added to the data store.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> was not assigned a unique identity value.</exception>
		/// <exception cref="ArgumentException">The data store already contains an entity with a matching unique identity value.</exception>
		void Add(TEntity item);

		/// <summary>
		/// Adds an entity to the data store, or updates it if it already exists in the data store.
		/// </summary>
		/// <param name="item">The entity to be added to or updated in the data store.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> was not assigned a unique identity value.</exception>
		void AddOrUpdate(TEntity item);

		/// <summary>
		/// Updates the specified entity in the data store.
		/// </summary>
		/// <param name="item">The entity to be updated in the data store.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> has a <c>null</c> unique identity value.</exception>
		/// <exception cref="KeyNotFoundException">The specified entity was not found in the data store.</exception>
		void Update(TEntity item);

		/// <summary>
		/// Removes the given entity from the data store.
		/// </summary>
		/// <param name="item">The entity to be removed from the data store.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> has a <c>null</c> unique identity value.</exception>
		/// <remarks>If the given entity is not found in the data store, this method is a no-op.</remarks>
		void Remove(TEntity item);

		/// <summary>
		/// Removes the specified entity from the data store.
		/// </summary>
		/// <param name="id">The unique identity value of the entity to be removed from the data store.</param>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		/// <remarks>If the specified entity is not found in the data store, this method is a no-op.</remarks>
		void Remove(TIdentity id);

		/// <summary>
		/// Attempts to add an entity to the data store.
		/// </summary>
		/// <param name="item">The entity to be added to the data store.</param>
		/// <returns><c>true</c> if the entity was added successfully; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> has a <c>null</c> unique identity value.</exception>
		bool TryAdd(TEntity item);

		/// <summary>
		/// Attempts to update the specified entity in the data store.
		/// </summary>
		/// <param name="item">The entity to be updated in the data store.</param>
		/// <returns><c>true</c> if the entity was updated successfully; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> has a <c>null</c> unique identity value.</exception>
		bool TryUpdate(TEntity item);

		/// <summary>
		/// Attempts to remove the given entity from the data store.
		/// </summary>
		/// <param name="item">The entity to be removed from the data store.</param>
		/// <returns><c>true</c> if the entity was removed successfully, or <c>false</c> if it was not found.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.
		/// -or- <paramref name="item"/> has a <c>null</c> unique identity value.</exception>
		/// <remarks>If the given entity is not found in the data store, this method is a no-op.</remarks>
		bool TryRemove(TEntity item);

		/// <summary>
		/// Attempts to remove the specified entity from the data store.
		/// </summary>
		/// <param name="id">The unique identity value of the entity to be removed from the data store.</param>
		/// <returns><c>true</c> if the entity was removed successfully, or <c>false</c> if it was not found.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		/// <remarks>If the specified entity is not found in the data store, this method is a no-op.</remarks>
		bool TryRemove(TIdentity id);
	}
}
