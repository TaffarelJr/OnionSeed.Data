using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class AsyncCommandServiceDecorator<TRoot, TIdentity> : IAsyncCommandService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncCommandServiceDecorator(IAsyncCommandService<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncCommandService{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public IAsyncCommandService<TRoot, TIdentity> Inner { get; }

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
