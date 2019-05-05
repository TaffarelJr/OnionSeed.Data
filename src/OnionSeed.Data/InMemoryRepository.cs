using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OnionSeed.Types;

namespace OnionSeed.Data
{
	/// <inheritdoc />
	/// <summary>
	/// A basic implementation of <see cref="IRepository{T,TId}"/> that simply stores data in an in-memory dictionary.
	/// </summary>
	/// <remarks>Since this repository is fast and contains no calls to external services,
	/// the functionality is implemented in a synchronous fashion.
	/// <p>This class is thread-safe.</p></remarks>
	[SuppressMessage("AsyncUsage.CSharp.Naming", "AvoidAsyncSuffix:Avoid Async suffix", Justification = "ValueTask is returned instead of Task.")]
	public class InMemoryRepository<T, TId> : IRepository<T, TId>
		where T : IEntity<TId>
		where TId : IEquatable<TId>, IComparable<TId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InMemoryRepository{T,TId}"/> class.
		/// </summary>
		public InMemoryRepository()
			: this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InMemoryRepository{T,TId}"/> class
		/// using the given list of entities as its initial data set.
		/// </summary>
		/// <param name="entities">The list of entities to be used as the initial data set.</param>
		/// <remarks>The list of entities is copied, so no references to it are kept internally.</remarks>
		public InMemoryRepository(IEnumerable<T> entities)
		{
			RawData = new ConcurrentDictionary<TId, T>(
				(entities ?? Enumerable.Empty<T>())
					.Select(e => new KeyValuePair<TId, T>(e.Id, e)));
		}

		/// <summary>
		/// Gets a reference to the raw data store.
		/// </summary>
		public ConcurrentDictionary<TId, T> RawData { get; }

#if COMPLETED_TASK
		private static Task CompletedTask => Task.CompletedTask;
#else
		private static Task CompletedTask => Task.FromResult(true);
#endif

		/// <inheritdoc />
		public ValueTask<int> GetCountAsync() => new ValueTask<int>(RawData.Count);

		/// <inheritdoc />
		public ValueTask<IEnumerable<T>> GetAllAsync() => new ValueTask<IEnumerable<T>>(RawData.Values);

		/// <inheritdoc />
		public ValueTask<T> GetByIdAsync(TId id)
		{
			ValidateId(id);

			if (RawData.TryGetValue(id, out var result))
				return new ValueTask<T>(result);

			throw new KeyNotFoundException($"Entity '{id}' was not found.");
		}

		/// <inheritdoc />
		public ValueTask<T> TryGetByIdAsync(TId id)
		{
			ValidateId(id);

			RawData.TryGetValue(id, out var result);
			return new ValueTask<T>(result);
		}

		/// <inheritdoc />
		public Task AddAsync(T item)
		{
			ValidateItemAndId(item);

			if (!RawData.TryAdd(item.Id, item))
				throw new ArgumentException($"Entity '{item.Id}' already exists.");

			return CompletedTask;
		}

		/// <inheritdoc />
		public Task AddOrUpdateAsync(T item)
		{
			ValidateItemAndId(item);
			RawData.AddOrUpdate(item.Id, item, (k, v) => item);
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task UpdateAsync(T item)
		{
			ValidateItemAndId(item);

			var success = false;
			while (!success)
			{
				if (!RawData.TryGetValue(item.Id, out var previous))
					throw new KeyNotFoundException($"Entity '{item.Id}' was not found.");

				success = RawData.TryUpdate(item.Id, item, previous);
			}

			return CompletedTask;
		}

		/// <inheritdoc />
		public Task RemoveAsync(T item)
		{
			ValidateItemAndId(item);
			RawData.TryRemove(item.Id, out var _);
			return CompletedTask;
		}

		/// <inheritdoc />
		public Task RemoveAsync(TId id)
		{
			ValidateId(id);
			RawData.TryRemove(id, out var _);
			return CompletedTask;
		}

		/// <inheritdoc />
		public ValueTask<bool> TryAddAsync(T item)
		{
			ValidateItemAndId(item);
			return new ValueTask<bool>(RawData.TryAdd(item.Id, item));
		}

		/// <inheritdoc />
		public ValueTask<bool> TryUpdateAsync(T item)
		{
			ValidateItemAndId(item);

			var success = false;
			while (!success)
			{
				if (!RawData.TryGetValue(item.Id, out var previous))
					return new ValueTask<bool>(false);

				success = RawData.TryUpdate(item.Id, item, previous);
			}

			return new ValueTask<bool>(true);
		}

		/// <inheritdoc />
		public ValueTask<bool> TryRemoveAsync(T item)
		{
			ValidateItemAndId(item);
			return new ValueTask<bool>(RawData.TryRemove(item.Id, out var _));
		}

		/// <inheritdoc />
		public ValueTask<bool> TryRemoveAsync(TId id)
		{
			ValidateId(id);
			return new ValueTask<bool>(RawData.TryRemove(id, out var _));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateId(TId id)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateItemAndId(T item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (item.Id == null)
				throw new ArgumentNullException(nameof(item), "Entity has a null ID.");
		}
	}
}
