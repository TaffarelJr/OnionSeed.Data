using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IQueryService{TEntity, TIdentity}"/> to work like an <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public class AsyncQueryServiceAdapter<TEntity, TIdentity> : QueryServiceDecorator<TEntity, TIdentity>, IAsyncQueryService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryServiceAdapter{TEntity, TIdentity}"/> class,
		/// wrapping the given <see cref="IQueryService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncQueryServiceAdapter(IQueryService<TEntity, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task<long> GetCountAsync() => Task.Run(() => Inner.GetCount());

		/// <inheritdoc/>
		public Task<IEnumerable<TEntity>> GetAllAsync() => Task.Run(() => Inner.GetAll());

		/// <inheritdoc/>
		public Task<TEntity> GetByIdAsync(TIdentity id) => Task.Run(() => Inner.GetById(id));

		/// <inheritdoc/>
		public Task<TEntity> TryGetByIdAsync(TIdentity id) => Task.Run(() => Inner.TryGetById(id, out TEntity result) ? result : default);
	}
}
