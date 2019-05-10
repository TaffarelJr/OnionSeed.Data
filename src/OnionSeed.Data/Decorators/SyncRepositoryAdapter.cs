using System;
using System.Collections.Generic;
using OnionSeed.Helpers.Async;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IAsyncRepository{TRoot, TIdentity}"/> to work like an <see cref="IRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public class SyncRepositoryAdapter<TRoot, TIdentity> : AsyncRepositoryDecorator<TRoot, TIdentity>, IRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncRepositoryAdapter{TRoot, TIdentity}"/> class,
		/// wrapping the given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public SyncRepositoryAdapter(IAsyncRepository<TRoot, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public long GetCount() => AsyncExtensions.RunSynchronously(() => Inner.GetCountAsync());

		/// <inheritdoc/>
		public IEnumerable<TRoot> GetAll() => AsyncExtensions.RunSynchronously(() => Inner.GetAllAsync());

		/// <inheritdoc/>
		public TRoot GetById(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.GetByIdAsync(id));

		/// <inheritdoc/>
		public bool TryGetById(TIdentity id, out TRoot result)
		{
			result = AsyncExtensions.RunSynchronously(() => Inner.TryGetByIdAsync(id));
			return !Equals(result, default(TRoot));
		}

		/// <inheritdoc/>
		public void Add(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.AddAsync(item));

		/// <inheritdoc/>
		public void AddOrUpdate(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.AddOrUpdateAsync(item));

		/// <inheritdoc/>
		public void Update(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.UpdateAsync(item));

		/// <inheritdoc/>
		public void Remove(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.RemoveAsync(item));

		/// <inheritdoc/>
		public void Remove(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.RemoveAsync(id));

		/// <inheritdoc/>
		public bool TryAdd(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.TryAddAsync(item));

		/// <inheritdoc/>
		public bool TryUpdate(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.TryUpdateAsync(item));

		/// <inheritdoc/>
		public bool TryRemove(TRoot item) => AsyncExtensions.RunSynchronously(() => Inner.TryRemoveAsync(item));

		/// <inheritdoc/>
		public bool TryRemove(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.TryRemoveAsync(id));
	}
}
