using System;
using System.Collections.Generic;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Joins an <see cref="IQueryService{TRoot, TIdentity}"/> and an <see cref="ICommandService{TRoot, TIdentity}"/> into a single <see cref="IRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public class ComposedRepository<TRoot, TIdentity> : IRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComposedRepository{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="queryService">The <see cref="IQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="ICommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public ComposedRepository(IQueryService<TRoot, TIdentity> queryService, ICommandService<TRoot, TIdentity> commandService)
		{
			QueryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
			CommandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IQueryService{TRoot, TIdentity}"/> being joined.
		/// </summary>
		public IQueryService<TRoot, TIdentity> QueryService { get; }

		/// <summary>
		/// Gets a reference to the <see cref="ICommandService{TRoot, TIdentity}"/> being joined.
		/// </summary>
		public ICommandService<TRoot, TIdentity> CommandService { get; }

		/// <inheritdoc/>
		public virtual long GetCount() => QueryService.GetCount();

		/// <inheritdoc/>
		public virtual IEnumerable<TRoot> GetAll() => QueryService.GetAll();

		/// <inheritdoc/>
		public virtual TRoot GetById(TIdentity id) => QueryService.GetById(id);

		/// <inheritdoc/>
		public virtual bool TryGetById(TIdentity id, out TRoot result) => QueryService.TryGetById(id, out result);

		/// <inheritdoc/>
		public virtual void Add(TRoot item) => CommandService.Add(item);

		/// <inheritdoc/>
		public virtual void AddOrUpdate(TRoot item) => CommandService.AddOrUpdate(item);

		/// <inheritdoc/>
		public virtual void Update(TRoot item) => CommandService.Update(item);

		/// <inheritdoc/>
		public virtual void Remove(TRoot item) => CommandService.Remove(item);

		/// <inheritdoc/>
		public virtual void Remove(TIdentity id) => CommandService.Remove(id);

		/// <inheritdoc/>
		public virtual bool TryAdd(TRoot item) => CommandService.TryAdd(item);

		/// <inheritdoc/>
		public virtual bool TryUpdate(TRoot item) => CommandService.TryUpdate(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TRoot item) => CommandService.TryRemove(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TIdentity id) => CommandService.TryRemove(id);
	}
}
