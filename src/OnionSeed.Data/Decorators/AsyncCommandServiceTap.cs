using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, mirroring commands to a secondary, "tap" <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
	/// </summary>
	/// <remarks>This decorator functions like a network tap: commands are executed first against the inner command service;
	/// if they succeed, they are then executed against the tap command service as well.
	/// <para>Any values returned or exceptions thrown from the tap command service are ignored.</para>
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class AsyncCommandServiceTap<TEntity, TIdentity> : AsyncCommandServiceDecorator<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTap{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTap(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap)
			: this(inner, tap, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTap{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTap(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, ILogger logger)
			: base(inner)
		{
			Logger = logger;
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					Logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' command service.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		public IAsyncCommandService<TEntity, TIdentity> Tap { get; }

		/// <summary>
		/// Gets a reference to the <see cref="ILogger"/>, if any, where tap exceptions should be written.
		/// </summary>
		public ILogger Logger { get; }

		/// <inheritdoc/>
		public override async Task AddAsync(TEntity item)
		{
			await Inner.AddAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task AddOrUpdateAsync(TEntity item)
		{
			await Inner.AddOrUpdateAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task UpdateAsync(TEntity item)
		{
			await Inner.UpdateAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TEntity item)
		{
			await Inner.RemoveAsync(item).ConfigureAwait(false);
			await Tap.RemoveAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TIdentity id)
		{
			await Inner.RemoveAsync(id).ConfigureAwait(false);
			await Tap.RemoveAsync(id).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task<bool> TryAddAsync(TEntity item)
		{
			var success = await Inner.TryAddAsync(item).ConfigureAwait(false);
			if (success)
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			return success;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryUpdateAsync(TEntity item)
		{
			var success = await Inner.TryUpdateAsync(item).ConfigureAwait(false);
			if (success)
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			return success;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TEntity item)
		{
			var success = await Inner.TryRemoveAsync(item).ConfigureAwait(false);
			await Tap.RemoveAsync(item).ConfigureAwait(false);
			return success;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TIdentity id)
		{
			var success = await Inner.TryRemoveAsync(id).ConfigureAwait(false);
			await Tap.RemoveAsync(id).ConfigureAwait(false);
			return success;
		}
	}
}
