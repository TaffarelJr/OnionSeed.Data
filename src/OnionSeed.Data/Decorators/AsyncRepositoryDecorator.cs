using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncRepository{TEntity, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncRepositoryDecorator<TEntity, TIdentity> : IAsyncRepository<TEntity, TIdentity>
		where TEntity : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncRepositoryDecorator{TEntity, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncRepository{TEntity, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncRepositoryDecorator(IAsyncRepository<TEntity, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncRepository{TEntity, TIdentity}"/> being decorated.
		/// </summary>
		public IAsyncRepository<TEntity, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => Inner.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TEntity>> GetAllAsync() => Inner.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TEntity> GetByIdAsync(TIdentity id) => Inner.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TEntity> TryGetByIdAsync(TIdentity id) => Inner.TryGetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task AddAsync(TEntity item) => Inner.AddAsync(item);

		/// <inheritdoc/>
		public virtual Task AddOrUpdateAsync(TEntity item) => Inner.AddOrUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task UpdateAsync(TEntity item) => Inner.UpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TEntity item) => Inner.RemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TIdentity id) => Inner.RemoveAsync(id);

		/// <inheritdoc/>
		public virtual Task<bool> TryAddAsync(TEntity item) => Inner.TryAddAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryUpdateAsync(TEntity item) => Inner.TryUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TEntity item) => Inner.TryRemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TIdentity id) => Inner.TryRemoveAsync(id);
	}
}
