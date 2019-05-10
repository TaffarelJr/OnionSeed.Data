using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncRepositoryDecorator<TRoot, TIdentity> : IAsyncRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncRepositoryDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncRepositoryDecorator(IAsyncRepository<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncRepository{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public IAsyncRepository<TRoot, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => Inner.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TRoot>> GetAllAsync() => Inner.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TRoot> GetByIdAsync(TIdentity id) => Inner.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TRoot> TryGetByIdAsync(TIdentity id) => Inner.TryGetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task AddAsync(TRoot item) => Inner.AddAsync(item);

		/// <inheritdoc/>
		public virtual Task AddOrUpdateAsync(TRoot item) => Inner.AddOrUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task UpdateAsync(TRoot item) => Inner.UpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TRoot item) => Inner.RemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TIdentity id) => Inner.RemoveAsync(id);

		/// <inheritdoc/>
		public virtual Task<bool> TryAddAsync(TRoot item) => Inner.TryAddAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryUpdateAsync(TRoot item) => Inner.TryUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TRoot item) => Inner.TryRemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TIdentity id) => Inner.TryRemoveAsync(id);
	}
}
