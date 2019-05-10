using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Joins an <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and an <see cref="IAsyncCommandService{TRoot, TIdentity}"/> into a single <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public class ComposedAsyncRepository<TRoot, TIdentity> : IAsyncRepository<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComposedAsyncRepository{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public ComposedAsyncRepository(IAsyncQueryService<TRoot, TIdentity> queryService, IAsyncCommandService<TRoot, TIdentity> commandService)
		{
			QueryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
			CommandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncQueryService{TRoot, TIdentity}"/> being joined.
		/// </summary>
		public IAsyncQueryService<TRoot, TIdentity> QueryService { get; }

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncCommandService{TRoot, TIdentity}"/> being joined.
		/// </summary>
		public IAsyncCommandService<TRoot, TIdentity> CommandService { get; }

		/// <inheritdoc/>
		public virtual Task<long> GetCountAsync() => QueryService.GetCountAsync();

		/// <inheritdoc/>
		public virtual Task<IEnumerable<TRoot>> GetAllAsync() => QueryService.GetAllAsync();

		/// <inheritdoc/>
		public virtual Task<TRoot> GetByIdAsync(TIdentity id) => QueryService.GetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task<TRoot> TryGetByIdAsync(TIdentity id) => QueryService.TryGetByIdAsync(id);

		/// <inheritdoc/>
		public virtual Task AddAsync(TRoot item) => CommandService.AddAsync(item);

		/// <inheritdoc/>
		public virtual Task AddOrUpdateAsync(TRoot item) => CommandService.AddOrUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task UpdateAsync(TRoot item) => CommandService.UpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TRoot item) => CommandService.RemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task RemoveAsync(TIdentity id) => CommandService.RemoveAsync(id);

		/// <inheritdoc/>
		public virtual Task<bool> TryAddAsync(TRoot item) => CommandService.TryAddAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryUpdateAsync(TRoot item) => CommandService.TryUpdateAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TRoot item) => CommandService.TryRemoveAsync(item);

		/// <inheritdoc/>
		public virtual Task<bool> TryRemoveAsync(TIdentity id) => CommandService.TryRemoveAsync(id);
	}
}
