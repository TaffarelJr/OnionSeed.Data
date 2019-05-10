using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IQueryService{TRoot, TIdentity}"/> to work like an <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public class AsyncQueryServiceAdapter<TRoot, TIdentity> : QueryServiceDecorator<TRoot, TIdentity>, IAsyncQueryService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryServiceAdapter{TRoot, TIdentity}"/> class,
		/// wrapping the given <see cref="IQueryService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncQueryServiceAdapter(IQueryService<TRoot, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task<long> GetCountAsync() => Task.Run(() => Inner.GetCount());

		/// <inheritdoc/>
		public Task<IEnumerable<TRoot>> GetAllAsync() => Task.Run(() => Inner.GetAll());

		/// <inheritdoc/>
		public Task<TRoot> GetByIdAsync(TIdentity id) => Task.Run(() => Inner.GetById(id));

		/// <inheritdoc/>
		public Task<TRoot> TryGetByIdAsync(TIdentity id) => Task.Run(() => Inner.TryGetById(id, out TRoot result) ? result : default);
	}
}
