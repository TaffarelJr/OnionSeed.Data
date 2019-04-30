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
	/// Any values returned or exceptions thrown from the tap command service are ignored.
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class AsyncCommandServiceTapDecorator<TEntity, TIdentity> : AsyncCommandServiceDecorator<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTapDecorator{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTapDecorator(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap)
			: this(inner, tap, null, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTapDecorator{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTapDecorator(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, ILogger logger)
			: this(inner, tap, logger, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTapDecorator{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="parallelMode">A value indicating whether the inner and tap should be called simultaneously.
		/// This behavior gives better performance, as both command services are executing in parallel. However it is
		/// also riskier because if an exception is thrown from one of them, they can become out-of-sync.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTapDecorator(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, bool parallelMode)
			: this(inner, tap, null, parallelMode)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTapDecorator{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <param name="parallelMode">A value indicating whether the inner and tap should be called simultaneously.
		/// This behavior gives better performance, as both command services are executing in parallel. However it is
		/// also riskier because if an exception is thrown from one of them, they can become out-of-sync.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTapDecorator(IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, ILogger logger, bool parallelMode)
			: base(inner)
		{
			ParallelMode = parallelMode;
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' command service.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		public IAsyncCommandService<TEntity, TIdentity> Tap { get; }

		/// <summary>
		/// Gets a value indicating whether the inner and tap should be called simultaneously.
		/// </summary>
		/// <remarks>This behavior gives better performance, as both command services are executing in parallel.
		/// However it is also riskier because if an exception is thrown from one of them, they can become out-of-sync.</remarks>
		public bool ParallelMode { get; }

		/// <inheritdoc/>
		public override async Task AddAsync(TEntity item)
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.AddAsync(item),
					Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
			}
			else
			{
				await Inner.AddAsync(item).ConfigureAwait(false);
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public override async Task AddOrUpdateAsync(TEntity item)
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.AddOrUpdateAsync(item),
					Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
			}
			else
			{
				await Inner.AddOrUpdateAsync(item).ConfigureAwait(false);
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public override async Task UpdateAsync(TEntity item)
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.UpdateAsync(item),
					Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
			}
			else
			{
				await Inner.UpdateAsync(item).ConfigureAwait(false);
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TEntity item)
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.RemoveAsync(item),
					Tap.RemoveAsync(item)).ConfigureAwait(false);
			}
			else
			{
				await Inner.RemoveAsync(item).ConfigureAwait(false);
				await Tap.RemoveAsync(item).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TIdentity id)
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.RemoveAsync(id),
					Tap.RemoveAsync(id)).ConfigureAwait(false);
			}
			else
			{
				await Inner.RemoveAsync(id).ConfigureAwait(false);
				await Tap.RemoveAsync(id).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public override async Task<bool> TryAddAsync(TEntity item)
		{
			if (ParallelMode)
			{
				var resultTask = Inner.TryAddAsync(item);
				await Task.WhenAll(resultTask, Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
				return resultTask.Result;
			}
			else
			{
				var success = await Inner.TryAddAsync(item).ConfigureAwait(false);
				if (success)
					await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
				return success;
			}
		}

		/// <inheritdoc/>
		public override async Task<bool> TryUpdateAsync(TEntity item)
		{
			if (ParallelMode)
			{
				var resultTask = Inner.TryUpdateAsync(item);
				await Task.WhenAll(resultTask, Tap.AddOrUpdateAsync(item)).ConfigureAwait(false);
				return resultTask.Result;
			}
			else
			{
				var success = await Inner.TryUpdateAsync(item).ConfigureAwait(false);
				if (success)
					await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
				return success;
			}
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TEntity item)
		{
			if (ParallelMode)
			{
				var resultTask = Inner.TryRemoveAsync(item);
				await Task.WhenAll(resultTask, Tap.RemoveAsync(item)).ConfigureAwait(false);
				return resultTask.Result;
			}
			else
			{
				var success = await Inner.TryRemoveAsync(item).ConfigureAwait(false);
				await Tap.RemoveAsync(item).ConfigureAwait(false);
				return success;
			}
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TIdentity id)
		{
			if (ParallelMode)
			{
				var resultTask = Inner.TryRemoveAsync(id);
				await Task.WhenAll(resultTask, Tap.RemoveAsync(id)).ConfigureAwait(false);
				return resultTask.Result;
			}
			else
			{
				var success = await Inner.TryRemoveAsync(id).ConfigureAwait(false);
				await Tap.RemoveAsync(id).ConfigureAwait(false);
				return success;
			}
		}
	}
}
