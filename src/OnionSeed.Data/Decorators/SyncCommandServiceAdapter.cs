using System;
using OnionSeed.Helpers.Async;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to work like an <see cref="ICommandService{TEntity, TIdentity}"/>.
	/// </summary>
	public class SyncCommandServiceAdapter<TEntity, TIdentity> : AsyncCommandServiceDecorator<TEntity, TIdentity>, ICommandService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncCommandServiceAdapter{TEntity, TIdentity}"/> class,
		/// wrapping the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public SyncCommandServiceAdapter(IAsyncCommandService<TEntity, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public void Add(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.AddAsync(item));

		/// <inheritdoc/>
		public void AddOrUpdate(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.AddOrUpdateAsync(item));

		/// <inheritdoc/>
		public void Update(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.UpdateAsync(item));

		/// <inheritdoc/>
		public void Remove(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.RemoveAsync(item));

		/// <inheritdoc/>
		public void Remove(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.RemoveAsync(id));

		/// <inheritdoc/>
		public bool TryAdd(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.TryAddAsync(item));

		/// <inheritdoc/>
		public bool TryUpdate(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.TryUpdateAsync(item));

		/// <inheritdoc/>
		public bool TryRemove(TEntity item) => AsyncExtensions.RunSynchronously(() => Inner.TryRemoveAsync(item));

		/// <inheritdoc/>
		public bool TryRemove(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.TryRemoveAsync(id));
	}
}
