﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, mirroring commands to a secondary, "tap" <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
	/// </summary>
	/// <remarks>This decorator functions like a network tap: commands are executed first against the inner command service;
	/// if they succeed, they are then executed against the tap command service as well.
	/// <para>Any values returned or exceptions thrown from the tap command service are ignored.</para>
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class AsyncCommandServiceTap<TRoot, TIdentity> : AsyncCommandServiceDecorator<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTap{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be duplicated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTap(IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap)
			: this(inner, tap, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandServiceTap{TRoot,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be duplicated.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncCommandServiceTap(IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap, ILogger logger)
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
			await Inner.AddAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task AddOrUpdateAsync(TRoot item)
		{
			await Inner.AddOrUpdateAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task UpdateAsync(TRoot item)
		{
			await Inner.UpdateAsync(item).ConfigureAwait(false);
			await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TRoot item)
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
		public override async Task<bool> TryAddAsync(TRoot item)
		{
			var success = await Inner.TryAddAsync(item).ConfigureAwait(false);
			if (success)
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			return success;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryUpdateAsync(TRoot item)
		{
			var success = await Inner.TryUpdateAsync(item).ConfigureAwait(false);
			if (success)
				await Tap.AddOrUpdateAsync(item).ConfigureAwait(false);
			return success;
		}

		/// <inheritdoc/>
		public override async Task<bool> TryRemoveAsync(TRoot item)
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
