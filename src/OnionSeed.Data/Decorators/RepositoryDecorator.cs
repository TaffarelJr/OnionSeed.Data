using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IRepository{TEntity, TIdentity}"/>.
	/// </summary>
	public abstract class RepositoryDecorator<TEntity, TIdentity> : IRepository<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryDecorator{TEntity, TIdentity}"/> class,
		/// decorating the given <see cref="IRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IRepository{TEntity, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public RepositoryDecorator(IRepository<TEntity, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IRepository{TEntity, TIdentity}"/> being decorated.
		/// </summary>
		public IRepository<TEntity, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => Inner.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TEntity> GetAll() => Inner.GetAll();

		/// <inheritdoc/>
		public virtual TEntity GetById(TIdentity id) => Inner.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TEntity result) => Inner.TryGetById(id, out result);

		/// <inheritdoc/>
		public virtual void Add(TEntity item) => Inner.Add(item);

		/// <inheritdoc/>
		public virtual void AddOrUpdate(TEntity item) => Inner.AddOrUpdate(item);

		/// <inheritdoc/>
		public virtual void Update(TEntity item) => Inner.Update(item);

		/// <inheritdoc/>
		public virtual void Remove(TEntity item) => Inner.Remove(item);

		/// <inheritdoc/>
		public virtual void Remove(TIdentity id) => Inner.Remove(id);

		/// <inheritdoc/>
		public virtual bool TryAdd(TEntity item) => Inner.TryAdd(item);

		/// <inheritdoc/>
		public virtual bool TryUpdate(TEntity item) => Inner.TryUpdate(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TEntity item) => Inner.TryRemove(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TIdentity id) => Inner.TryRemove(id);
	}
}
