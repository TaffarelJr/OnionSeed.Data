using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IRepository{TRoot, TIdentity}"/> to work like an <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public class AsyncRepositoryAdapter<TRoot, TIdentity> : RepositoryDecorator<TRoot, TIdentity>, IAsyncRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncRepositoryAdapter{TRoot, TIdentity}"/> class,
		/// wrapping the given <see cref="IRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncRepositoryAdapter(IRepository<TRoot, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task<long> GetCountAsync() => Task.Run(() => Inner.GetCount());

		/// <inheritdoc/>
		public Task<IEnumerable<TRoot>> GetAllAsync() => Task.Run(() => Inner.GetAll());

		/// <inheritdoc/>
		public Task<TRoot> GetByIdAsync(TIdentity id) => Task.Run(() => Inner.GetById(id));

		/// <inheritdoc/>
		public Task<TRoot> TryGetByIdAsync(TIdentity id) => Task.Run(() => Inner.TryGetById(id, out TRoot result) ? result : default);

		/// <inheritdoc/>
		public Task AddAsync(TRoot item) => Task.Run(() => Inner.Add(item));

		/// <inheritdoc/>
		public Task AddOrUpdateAsync(TRoot item) => Task.Run(() => Inner.AddOrUpdate(item));

		/// <inheritdoc/>
		public Task UpdateAsync(TRoot item) => Task.Run(() => Inner.Update(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TRoot item) => Task.Run(() => Inner.Remove(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TIdentity id) => Task.Run(() => Inner.Remove(id));

		/// <inheritdoc/>
		public Task<bool> TryAddAsync(TRoot item) => Task.Run(() => Inner.TryAdd(item));

		/// <inheritdoc/>
		public Task<bool> TryUpdateAsync(TRoot item) => Task.Run(() => Inner.TryUpdate(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TRoot item) => Task.Run(() => Inner.TryRemove(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TIdentity id) => Task.Run(() => Inner.TryRemove(id));
	}
}
