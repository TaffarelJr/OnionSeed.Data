﻿using System;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IRepository{TEntity, TIdentity}"/>, mirroring commands to a secondary, "tap" <see cref="IRepository{TEntity, TIdentity}"/>.
	/// </summary>
	/// <remarks>This decorator functions like a network tap: commands are executed first against the inner repository;
	/// if they succeed, they are then executed against the tap repository as well.
	/// Any values returned or exceptions thrown from the tap repository are ignored.
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class RepositoryTapDecorator<TEntity, TIdentity> : RepositoryDecorator<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryTapDecorator{TEntity,TKey}"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IRepository{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IRepository{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The logger where tap exceptions should be written. Optional.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public RepositoryTapDecorator(IRepository<TEntity, TIdentity> inner, IRepository<TEntity, TIdentity> tap, ILogger logger = null)
			: base(inner)
		{
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' repository.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IRepository{TEntity, TIdentity}"/>.
		/// </summary>
		public IRepository<TEntity, TIdentity> Tap { get; }

		/// <inheritdoc/>
		public override void Add(TEntity item)
		{
			Inner.Add(item);
			Tap.AddOrUpdate(item);
		}

		/// <inheritdoc/>
		public override void AddOrUpdate(TEntity item)
		{
			Inner.AddOrUpdate(item);
			Tap.AddOrUpdate(item);
		}

		/// <inheritdoc/>
		public override void Update(TEntity item)
		{
			Inner.Update(item);
			Tap.AddOrUpdate(item);
		}

		/// <inheritdoc/>
		public override void Remove(TEntity item)
		{
			Inner.Remove(item);
			Tap.Remove(item);
		}

		/// <inheritdoc/>
		public override void Remove(TIdentity id)
		{
			Inner.Remove(id);
			Tap.Remove(id);
		}

		/// <inheritdoc/>
		public override bool TryAdd(TEntity item)
		{
			var success = Inner.TryAdd(item);
			if (success)
				Tap.AddOrUpdate(item);
			return success;
		}

		/// <inheritdoc/>
		public override bool TryUpdate(TEntity item)
		{
			var success = Inner.TryUpdate(item);
			if (success)
				Tap.AddOrUpdate(item);
			return success;
		}

		/// <inheritdoc/>
		public override bool TryRemove(TEntity item)
		{
			var success = Inner.TryRemove(item);
			Tap.Remove(item);
			return success;
		}

		/// <inheritdoc/>
		public override bool TryRemove(TIdentity id)
		{
			var success = Inner.TryRemove(id);
			Tap.Remove(id);
			return success;
		}
	}
}
