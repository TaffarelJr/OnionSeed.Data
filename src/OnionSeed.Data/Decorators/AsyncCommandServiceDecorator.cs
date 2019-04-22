using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncCommandServiceDecorator<TEntity, TIdentity> : IAsyncCommandService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceDecorator{TEntity, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncCommandServiceDecorator(IAsyncCommandService<TEntity, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncCommandService{TEntity, TIdentity}"/> being decorated.
		/// </summary>
		protected IAsyncCommandService<TEntity, TIdentity> Inner { get; }

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
