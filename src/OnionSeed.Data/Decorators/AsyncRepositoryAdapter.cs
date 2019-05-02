using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IRepository{TEntity, TIdentity}"/> to work like an <see cref="IAsyncRepository{TEntity, TIdentity}"/>.
	/// </summary>
	public class AsyncRepositoryAdapter<TEntity, TIdentity> : RepositoryDecorator<TEntity, TIdentity>, IAsyncRepository<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncRepositoryAdapter{TEntity, TIdentity}"/> class,
		/// wrapping the given <see cref="IRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IRepository{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncRepositoryAdapter(IRepository<TEntity, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task<long> GetCountAsync() => Task.Run(() => Inner.GetCount());

		/// <inheritdoc/>
		public Task<IEnumerable<TEntity>> GetAllAsync() => Task.Run(() => Inner.GetAll());

		/// <inheritdoc/>
		public Task<TEntity> GetByIdAsync(TIdentity id) => Task.Run(() => Inner.GetById(id));

		/// <inheritdoc/>
		public Task<TEntity> TryGetByIdAsync(TIdentity id) => Task.Run(() => Inner.TryGetById(id, out TEntity result) ? result : default);

		/// <inheritdoc/>
		public Task AddAsync(TEntity item) => Task.Run(() => Inner.Add(item));

		/// <inheritdoc/>
		public Task AddOrUpdateAsync(TEntity item) => Task.Run(() => Inner.AddOrUpdate(item));

		/// <inheritdoc/>
		public Task UpdateAsync(TEntity item) => Task.Run(() => Inner.Update(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TEntity item) => Task.Run(() => Inner.Remove(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TIdentity id) => Task.Run(() => Inner.Remove(id));

		/// <inheritdoc/>
		public Task<bool> TryAddAsync(TEntity item) => Task.Run(() => Inner.TryAdd(item));

		/// <inheritdoc/>
		public Task<bool> TryUpdateAsync(TEntity item) => Task.Run(() => Inner.TryUpdate(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TEntity item) => Task.Run(() => Inner.TryRemove(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TIdentity id) => Task.Run(() => Inner.TryRemove(id));
	}
}
