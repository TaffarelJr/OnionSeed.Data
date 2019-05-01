using System;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IUnitOfWork"/>, mirroring commands to a secondary, "tap" <see cref="IUnitOfWork"/>.
	/// </summary>
	/// <remarks>This decorator functions like a network tap: commands are executed first against the inner unit of work;
	/// if they succeed, they are then executed against the tap unit of work as well.
	/// <para>Any values returned or exceptions thrown from the tap unit of work are ignored.</para>
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class UnitOfWorkTap : UnitOfWorkDecorator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkTap"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IUnitOfWork"/>, where commands will be mirrored.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public UnitOfWorkTap(IUnitOfWork inner, IUnitOfWork tap)
			: this(inner, tap, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkTap"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public UnitOfWorkTap(IUnitOfWork inner, IUnitOfWork tap, ILogger logger)
			: base(inner)
		{
			Logger = logger;
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					Logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' unit of work.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IUnitOfWork"/>.
		/// </summary>
		public IUnitOfWork Tap { get; }

		/// <summary>
		/// Gets a reference to the <see cref="ILogger"/>, if any, where tap exceptions should be written.
		/// </summary>
		public ILogger Logger { get; }

		/// <inheritdoc/>
		public override void Commit()
		{
			Inner.Commit();
			Tap.Commit();
		}
	}
}
