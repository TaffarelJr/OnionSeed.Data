using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ApplCore.Types;

namespace ApplCore.DataAccess
{
	/// <summary>
	/// A simple implementation of <see cref="IRepository{TRoot,TKey}"/> that stores objects in-memory.
	/// </summary>
	/// <typeparam name="TRoot">The type of items that are stored in the repository.</typeparam>
	/// <typeparam name="TKey">The type of the unique identity value.</typeparam>
	public class InMemoryDataStore<TRoot, TKey> : RepositoryBase<TRoot, TKey>, IQueryService<TRoot, TKey>
		where TRoot : class, IAggregateRoot<TKey>
		where TKey : IComparable<TKey>, IEquatable<TKey>
	{
		/// <summary>
		/// The dictionary where the items will be stored.
		/// </summary>
		private readonly IDictionary<TKey, TRoot> _data;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public InMemoryDataStore()
			: this(new Dictionary<TKey, TRoot>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the class using the given dictionary as its storage medium.
		/// </summary>
		/// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> to use as the storage medium.</param>
		public InMemoryDataStore(IDictionary<TKey, TRoot> dictionary)
		{
			Contract.Requires(dictionary != null);
			Contract.Requires(Contract.ForAll(dictionary, p => p.Value != null));
			_data = dictionary;
		}

		#region IRepository implementation

		/// <summary>
		/// Attempts to insert a new item into the repository and returns the result.
		/// </summary>
		/// <param name="item">The item to be inserted.</param>
		/// <returns><b>true</b> if the item was inserted successfully, or <b>false</b> if it already exists in the repository.</returns>
		public override bool TryInsert(TRoot item)
		{
			if (_data.ContainsKey(item.Id))
			{
				return false;
			}

			_data.Add(item.Id, item);
			return true;
		}

		/// <summary>
		/// Attempts to update an existing item in the repository and returns the result.
		/// </summary>
		/// <param name="item">The item to be updated.</param>
		/// <returns><b>true</b> if the item was updated successfully, or <b>false</b> if it does not exist in the repository.</returns>
		public override bool TryUpdate(TRoot item)
		{
			if (!_data.ContainsKey(item.Id))
			{
				return false;
			}

			_data[item.Id] = item;
			return true;
		}

		/// <summary>
		/// Attempts to remove an existing item in the repository and returns the result.
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <returns><b>true</b> if the item was removed successfully, or <b>false</b> if it does not exist in the repository.</returns>
		public override bool TryRemove(TRoot item)
		{
			return _data.Remove(item.Id);
		}

		#endregion

		#region IQueryService Implementation

		/// <summary>
		/// Returns the total number of items in the collection.
		/// </summary>
		/// <returns>The total number of items in the collection.</returns>
		public int GetCount()
		{
			return _data.Count;
		}

		/// <summary>
		/// Determines whether the collection contains a specific item.
		/// </summary>
		/// <param name="item">The item to locate in the collection.</param>
		/// <returns><b>true</b> if <paramref name="item"/> is found in the collection; otherwise, <b>false</b>.</returns>
		public bool Contains(TRoot item)
		{
			return _data.ContainsKey(item.Id);
		}

		/// <summary>
		/// Returns an enumeration of the items in the collection.
		/// </summary>
		/// <returns>An enumeration of the items in the collection.</returns>
		public IEnumerable<TRoot> StreamAll()
		{
			return _data.Values;
		}

		/// <summary>
		/// Attempts to find the object that has the given unique identity value.
		/// </summary>
		/// <param name="id">The unique identity value of the object.</param>
		/// <returns>The object with the given unique identity value, or <b>null</b> if no object was found with the given unique identity value.</returns>
		public TRoot GetById(TKey id)
		{
			return _data.ContainsKey(id) ? _data[id] : null;
		}

		#endregion

		/// <summary>
		/// Specifies the Code Contract invariants for the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(_data != null);
		}
	}
}