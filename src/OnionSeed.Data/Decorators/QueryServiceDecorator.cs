using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public abstract class QueryServiceDecorator<TEntity, TIdentity> : IQueryService<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryServiceDecorator{TEntity, TIdentity}"/> class,
		/// decorating the given <see cref="IQueryService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IQueryService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public QueryServiceDecorator(IQueryService<TEntity, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IQueryService{TEntity, TIdentity}"/> being decorated.
		/// </summary>
		public IQueryService<TEntity, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => Inner.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TEntity> GetAll() => Inner.GetAll();

		/// <inheritdoc/>
		public virtual TEntity GetById(TIdentity id) => Inner.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TEntity result) => Inner.TryGetById(id, out result);
	}
}
