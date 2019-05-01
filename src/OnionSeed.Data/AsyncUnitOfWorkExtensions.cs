using System;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncUnitOfWork"/>.
	/// </summary>
	public static class AsyncUnitOfWorkExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a <see cref="AsyncUnitOfWorkExceptionHandler{TException}"/>.
		/// </summary>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkExceptionHandler{TException}"/> wrapping the given <see cref="IAsyncUnitOfWork"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork Catch<TException>(this IAsyncUnitOfWork inner, Func<TException, bool> handler)
			where TException : Exception
		{
			return new AsyncUnitOfWorkExceptionHandler<TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a sequential <see cref="AsyncUnitOfWorkTap"/>,
		/// mirroring all commands to the other given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkTap"/> wrapping the two given <see cref="IAsyncUnitOfWork"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork WithSequentialTap(this IAsyncUnitOfWork inner, IAsyncUnitOfWork tap)
		{
			return new AsyncUnitOfWorkTap(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a sequential <see cref="AsyncUnitOfWorkTap"/>,
		/// mirroring all commands to the other given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkTap"/> wrapping the two given <see cref="IAsyncUnitOfWork"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork WithSequentialTap(this IAsyncUnitOfWork inner, IAsyncUnitOfWork tap, ILogger logger)
		{
			return new AsyncUnitOfWorkTap(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a parallel <see cref="AsyncUnitOfWorkTap"/>,
		/// mirroring all commands to the other given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkTap"/> wrapping the two given <see cref="IAsyncUnitOfWork"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork WithParallelTap(this IAsyncUnitOfWork inner, IAsyncUnitOfWork tap)
		{
			return new AsyncUnitOfWorkParallelTap(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a parallel <see cref="AsyncUnitOfWorkTap"/>,
		/// mirroring all commands to the other given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncUnitOfWork"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkTap"/> wrapping the two given <see cref="IAsyncUnitOfWork"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork WithParallelTap(this IAsyncUnitOfWork inner, IAsyncUnitOfWork tap, ILogger logger)
		{
			return new AsyncUnitOfWorkParallelTap(inner, tap, logger);
		}
	}
}
