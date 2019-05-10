using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="ICommandService{TRoot, TIdentity}"/> to work like an <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
	/// </summary>
	public class AsyncCommandServiceAdapter<TRoot, TIdentity> : CommandServiceDecorator<TRoot, TIdentity>, IAsyncCommandService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceAdapter{TRoot, TIdentity}"/> class,
		/// wrapping the given <see cref="ICommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncCommandServiceAdapter(ICommandService<TRoot, TIdentity> inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task AddAsync(TRoot item) => Task.Run(() => Inner.Add(item));

		/// <inheritdoc/>
		public Task AddOrUpdateAsync(TRoot item) => Task.Run(() => Inner.AddOrUpdate(item));

		/// <inheritdoc/>
		public Task UpdateAsync(TRoot item) => Task.Run(() => Inner.Update(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TRoot item) => Task.Run(() => Inner.Remove(item));

		/// <inheritdoc/>
		public Task RemoveAsync(TIdentity id) => Task.Run(() => Inner.Remove(id));

		/// <inheritdoc/>
		public Task<bool> TryAddAsync(TRoot item) => Task.Run(() => Inner.TryAdd(item));

		/// <inheritdoc/>
		public Task<bool> TryUpdateAsync(TRoot item) => Task.Run(() => Inner.TryUpdate(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TRoot item) => Task.Run(() => Inner.TryRemove(item));

		/// <inheritdoc/>
		public Task<bool> TryRemoveAsync(TIdentity id) => Task.Run(() => Inner.TryRemove(id));
	}
}
