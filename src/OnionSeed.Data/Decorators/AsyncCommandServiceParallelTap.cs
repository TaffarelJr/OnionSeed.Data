using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, mirroring commands to a secondary, "tap" <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
	/// </summary>
	/// <remarks>This decorator functions like a parallel network tap: commands are executed against the inner command service
	/// and the tap command service at the same time. This can be more performant than the regular, sequential type of tap;
	/// but it is also inherently riskier because if an exception is thrown from either command service, they can become out-of-sync.
	/// <para>Any values returned or exceptions thrown from the tap command service are ignored.</para>
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class AsyncCommandServiceParallelTap<TRoot, TIdentity> : AsyncCommandServiceDecorator<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceParallelTap{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be duplicated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceParallelTap(IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap)
			: this(inner, tap, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceParallelTap{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceParallelTap(IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap, ILogger logger)
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
		/// Gets a reference to the tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		public IAsyncCommandService<TRoot, TIdentity> Tap { get; }

		/// <summary>
		/// Gets a reference to the <see cref="ILogger"/>, if any, where tap exceptions should be written.
		/// </summary>
		public ILogger Logger { get; }

		/// <inheritdoc/>
		public override async Task AddAsync(TRoot item)
		{
			await Task.WhenAll(
				Inner.AddAsync(item),
				Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task AddOrUpdateAsync(TRoot item)
		{
			await Task.WhenAll(
				Inner.AddOrUpdateAsync(item),
				Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task UpdateAsync(TRoot item)
		{
			await Task.WhenAll(
				Inner.UpdateAsync(item),
				Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TRoot item)
		{
			await Task.WhenAll(
				Inner.RemoveAsync(item),
				Tap.RemoveAsync(item)).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TIdentity id)
		{
			await Task.WhenAll(
				Inner.RemoveAsync(id),
				Tap.RemoveAsync(id)).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task<bool> TryAddAsync(TRoot item)
		{
			var resultTask = Inner.TryAddAsync(item);
			await Task.WhenAll(resultTask, Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
			return resultTask.Result;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryUpdateAsync(TRoot item)
		{
			var resultTask = Inner.TryUpdateAsync(item);
			await Task.WhenAll(resultTask, Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
			return resultTask.Result;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TRoot item)
		{
			var resultTask = Inner.TryRemoveAsync(item);
			await Task.WhenAll(resultTask, Tap.RemoveAsync(item)).ConfigureAwait(false);
			return resultTask.Result;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TIdentity id)
		{
			var resultTask = Inner.TryRemoveAsync(id);
			await Task.WhenAll(resultTask, Tap.RemoveAsync(id)).ConfigureAwait(false);
			return resultTask.Result;
		}
	}
}
