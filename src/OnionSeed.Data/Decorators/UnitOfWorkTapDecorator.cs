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
	/// Any values returned or exceptions thrown from the tap unit of work are ignored.
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class UnitOfWorkTapDecorator : UnitOfWorkDecorator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkTapDecorator"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The logger where tap exceptions should be written. Optional.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public UnitOfWorkTapDecorator(IUnitOfWork inner, IUnitOfWork tap, ILogger logger = null)
			: base(inner)
		{
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' unit of work.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IUnitOfWork"/>.
		/// </summary>
		public IUnitOfWork Tap { get; }

		/// <inheritdoc/>
		public override void Commit()
		{
			Inner.Commit();
			Tap.Commit();
		}
	}
}
