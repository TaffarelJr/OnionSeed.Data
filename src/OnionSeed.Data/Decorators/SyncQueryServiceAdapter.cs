using System;
using System.Collections.Generic;
using OnionSeed.Helpers.Async;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to work like an <see cref="IQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public class SyncQueryServiceAdapter<TRoot, TIdentity> : AsyncQueryServiceDecorator<TRoot, TIdentity>, IQueryService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncQueryServiceAdapter{TRoot, TIdentity}"/> class,
		/// wrapping the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public SyncQueryServiceAdapter(IAsyncQueryService<TRoot, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public long GetCount() => AsyncExtensions.RunSynchronously(() => Inner.GetCountAsync());

		/// <inheritdoc/>
		public IEnumerable<TRoot> GetAll() => AsyncExtensions.RunSynchronously(() => Inner.GetAllAsync());

		/// <inheritdoc/>
		public TRoot GetById(TIdentity id) => AsyncExtensions.RunSynchronously(() => Inner.GetByIdAsync(id));

		/// <inheritdoc/>
		public bool TryGetById(TIdentity id, out TRoot result)
		{
			result = AsyncExtensions.RunSynchronously(() => Inner.TryGetByIdAsync(id));
			return !Equals(result, default(TRoot));
		}
	}
}
