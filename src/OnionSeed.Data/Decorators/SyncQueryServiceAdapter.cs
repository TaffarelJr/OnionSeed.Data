using System;
using System.Collections.Generic;
using OnionSeed.Helpers.Async;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to work like an <see cref="IQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public class SyncQueryServiceAdapter<TEntity, TIdentity> : AsyncQueryServiceDecorator<TEntity, TIdentity>, IQueryService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncQueryServiceAdapter{TEntity, TIdentity}"/> class,
		/// wrapping the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public SyncQueryServiceAdapter(IAsyncQueryService<TEntity, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public long GetCount() => AsyncExtensions.RunSynchronously(() => Inner.GetCountAsync());

		/// <inheritdoc/>
		public IEnumerable<TEntity> GetAll() => AsyncExtensions.RunSynchronously(() => Inner.GetAllAsync());

		/// <inheritdoc/>
		public TEntity GetById(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.GetByIdAsync(id));

		/// <inheritdoc/>
		public bool TryGetById(TIdentity id, out TEntity result)
		{
			result = AsyncExtensions.RunSynchronously(() => Inner.TryGetByIdAsync(id));
			return !Equals(result, default(TEntity));
		}
	}
}
