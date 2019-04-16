using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncQueryServiceDecorator<TEntity, TIdentity> : IAsyncQueryService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryServiceDecorator{TEntity, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncQueryServiceDecorator(IAsyncQueryService<TEntity, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncQueryService{TEntity, TIdentity}"/> being decorated.
		/// </summary>
		protected IAsyncQueryService<TEntity, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => Inner.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TEntity>> GetAllAsync() => Inner.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TEntity> GetByIdAsync(TIdentity id) => Inner.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TEntity> TryGetByIdAsync(TIdentity id) => Inner.TryGetByIdAsync(id);
	}
}
