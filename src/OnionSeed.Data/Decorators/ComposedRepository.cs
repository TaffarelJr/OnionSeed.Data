using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Joins an <see cref="IQueryService{TEntity, TIdentity}"/> and an <see cref="ICommandService{TEntity, TIdentity}"/> into a single <see cref="IRepository{TEntity, TIdentity}"/>.
	/// </summary>
	public class ComposedRepository<TEntity, TIdentity> : IRepository<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComposedRepository{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="queryService">The <see cref="IQueryService{TEntity, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="ICommandService{TEntity, TIdentity}"/>to be joined.</param>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public ComposedRepository(IQueryService<TEntity, TIdentity> queryService, ICommandService<TEntity, TIdentity> commandService)
		{
			QueryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
			CommandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IQueryService{TEntity, TIdentity}"/> being joined.
		/// </summary>
		public IQueryService<TEntity, TIdentity> QueryService { get; }

		/// <summary>
		/// Gets a reference to the <see cref="ICommandService{TEntity, TIdentity}"/> being joined.
		/// </summary>
		public ICommandService<TEntity, TIdentity> CommandService { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => QueryService.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TEntity> GetAll() => QueryService.GetAll();

		/// <inheritdoc/>
		public virtual TEntity GetById(TIdentity id) => QueryService.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TEntity result) => QueryService.TryGetById(id, out result);

		/// <inheritdoc/>
		public virtual void Add(TEntity item) => CommandService.Add(item);

		/// <inheritdoc/>
		public virtual void AddOrUpdate(TEntity item) => CommandService.AddOrUpdate(item);

		/// <inheritdoc/>
		public virtual void Update(TEntity item) => CommandService.Update(item);

		/// <inheritdoc/>
		public virtual void Remove(TEntity item) => CommandService.Remove(item);

		/// <inheritdoc/>
		public virtual void Remove(TIdentity id) => CommandService.Remove(id);

		/// <inheritdoc/>
		public virtual bool TryAdd(TEntity item) => CommandService.TryAdd(item);

		/// <inheritdoc/>
		public virtual bool TryUpdate(TEntity item) => CommandService.TryUpdate(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TEntity item) => CommandService.TryRemove(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TIdentity id) => CommandService.TryRemove(id);
	}
}
