using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class RepositoryDecorator<TRoot, TIdentity> : IRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="IRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public RepositoryDecorator(IRepository<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IRepository{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public IRepository<TRoot, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => Inner.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TRoot> GetAll() => Inner.GetAll();

		/// <inheritdoc/>
		public virtual TRoot GetById(TIdentity id) => Inner.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TRoot result) => Inner.TryGetById(id, out result);

		/// <inheritdoc/>
		public virtual void Add(TRoot item) => Inner.Add(item);

		/// <inheritdoc/>
		public virtual void AddOrUpdate(TRoot item) => Inner.AddOrUpdate(item);

		/// <inheritdoc/>
		public virtual void Update(TRoot item) => Inner.Update(item);

		/// <inheritdoc/>
		public virtual void Remove(TRoot item) => Inner.Remove(item);

		/// <inheritdoc/>
		public virtual void Remove(TIdentity id) => Inner.Remove(id);

		/// <inheritdoc/>
		public virtual bool TryAdd(TRoot item) => Inner.TryAdd(item);

		/// <inheritdoc/>
		public virtual bool TryUpdate(TRoot item) => Inner.TryUpdate(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TRoot item) => Inner.TryRemove(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TIdentity id) => Inner.TryRemove(id);
	}
}
