using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Joins an <see cref="IAsyncQueryService{TEntity, TIdentity}"/> and an <see cref="IAsyncCommandService{TEntity, TIdentity}"/> into a single <see cref="IAsyncRepository{TEntity, TIdentity}"/>.
	/// </summary>
	public class ComposedAsyncRepository<TEntity, TIdentity> : IAsyncRepository<TEntity, TIdentity>
		where TEntity : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComposedAsyncRepository{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/>to be joined.</param>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public ComposedAsyncRepository(IAsyncQueryService<TEntity, TIdentity> queryService, IAsyncCommandService<TEntity, TIdentity> commandService)
		{
			QueryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
			CommandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncQueryService{TEntity, TIdentity}"/> being joined.
		/// </summary>
		public IAsyncQueryService<TEntity, TIdentity> QueryService { get; }

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncCommandService{TEntity, TIdentity}"/> being joined.
		/// </summary>
		public IAsyncCommandService<TEntity, TIdentity> CommandService { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => QueryService.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TEntity>> GetAllAsync() => QueryService.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TEntity> GetByIdAsync(TIdentity id) => QueryService.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TEntity> TryGetByIdAsync(TIdentity id) => QueryService.TryGetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task AddAsync(TEntity item) => CommandService.AddAsync(item);

		/// <inheritdoc/>
		public virtual Task AddOrUpdateAsync(TEntity item) => CommandService.AddOrUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task UpdateAsync(TEntity item) => CommandService.UpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TEntity item) => CommandService.RemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TIdentity id) => CommandService.RemoveAsync(id);

		/// <inheritdoc/>
		public virtual Task<bool> TryAddAsync(TEntity item) => CommandService.TryAddAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryUpdateAsync(TEntity item) => CommandService.TryUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TEntity item) => CommandService.TryRemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TIdentity id) => CommandService.TryRemoveAsync(id);
	}
}
