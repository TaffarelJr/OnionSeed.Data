using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class QueryServiceDecorator<TRoot, TIdentity> : IQueryService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryServiceDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="IQueryService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IQueryService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public QueryServiceDecorator(IQueryService<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IQueryService{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public IQueryService<TRoot, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => Inner.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TRoot> GetAll() => Inner.GetAll();

		/// <inheritdoc/>
		public virtual TRoot GetById(TIdentity id) => Inner.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TRoot result) => Inner.TryGetById(id, out result);
	}
}
