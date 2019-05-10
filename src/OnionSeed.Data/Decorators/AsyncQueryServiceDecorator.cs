using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncQueryServiceDecorator<TRoot, TIdentity> : IAsyncQueryService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryServiceDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncQueryServiceDecorator(IAsyncQueryService<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncQueryService{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public IAsyncQueryService<TRoot, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => Inner.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TRoot>> GetAllAsync() => Inner.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TRoot> GetByIdAsync(TIdentity id) => Inner.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TRoot> TryGetByIdAsync(TIdentity id) => Inner.TryGetByIdAsync(id);
	}
}
