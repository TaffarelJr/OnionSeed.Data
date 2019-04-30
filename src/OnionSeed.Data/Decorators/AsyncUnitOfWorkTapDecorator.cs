using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Decorates an <see cref="IAsyncUnitOfWork"/>, mirroring commands to a secondary, "tap" <see cref="IAsyncUnitOfWork"/>.
	/// </summary>
	/// <remarks>This decorator functions like a network tap: commands are executed first against the inner unit of work;
	/// if they succeed, they are then executed against the tap unit of work as well.
	/// Any values returned or exceptions thrown from the tap unit of work are ignored.
	/// <para>This essentially allows for the creation of a duplicate copy of the data,
	/// and is intended to be used for things like caching, backup, or reporting.</para></remarks>
	public class AsyncUnitOfWorkTapDecorator : AsyncUnitOfWorkDecorator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkTapDecorator"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkTapDecorator(IAsyncUnitOfWork inner, IAsyncUnitOfWork tap)
			: this(inner, tap, null, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkTapDecorator"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkTapDecorator(IAsyncUnitOfWork inner, IAsyncUnitOfWork tap, ILogger logger)
			: this(inner, tap, logger, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkTapDecorator"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="parallelMode">A value indicating whether the inner and tap should be called simultaneously.
		/// This behavior gives better performance, as both units of work are executing in parallel. However it is
		/// also riskier because if an exception is thrown from one of them, they can become out-of-sync.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkTapDecorator(IAsyncUnitOfWork inner, IAsyncUnitOfWork tap, bool parallelMode)
			: this(inner, tap, null, parallelMode)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkTapDecorator"/> class.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The logger where tap exceptions should be written.</param>
		/// <param name="parallelMode">A value indicating whether the inner and tap should be called simultaneously.
		/// This behavior gives better performance, as both units of work are executing in parallel. However it is
		/// also riskier because if an exception is thrown from one of them, they can become out-of-sync.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkTapDecorator(IAsyncUnitOfWork inner, IAsyncUnitOfWork tap, ILogger logger, bool parallelMode)
			: base(inner)
		{
			ParallelMode = parallelMode;
			Tap = (tap ?? throw new ArgumentNullException(nameof(tap)))
				.Catch((Exception ex) =>
				{
					logger?.LogWarning(0, ex, "An exception ocurred in the 'tap' unit of work.");
					return true;
				});
		}

		/// <summary>
		/// Gets a reference to the tap <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		public IAsyncUnitOfWork Tap { get; }

		/// <summary>
		/// Gets a value indicating whether the inner and tap should be called simultaneously.
		/// </summary>
		/// <remarks>This behavior gives better performance, as both units of work are executing in parallel.
		/// However it is also riskier because if an exception is thrown from one of them, they can become out-of-sync.</remarks>
		public bool ParallelMode { get; }

		/// <inheritdoc/>
		public override async Task CommitAsync()
		{
			if (ParallelMode)
			{
				await Task.WhenAll(
					Inner.CommitAsync(),
					Tap.CommitAsync()).ConfigureAwait(false);
			}
			else
			{
				await Inner.CommitAsync().ConfigureAwait(false);
				await Tap.CommitAsync().ConfigureAwait(false);
			}
		}
	}
}
