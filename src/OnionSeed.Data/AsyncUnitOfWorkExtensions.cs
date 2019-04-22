using System;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncUnitOfWork"/>.
	/// </summary>
	public static class AsyncUnitOfWorkExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncUnitOfWork"/> in a <see cref="AsyncUnitOfWorkExceptionHandlerDecorator{TException}"/>.
		/// </summary>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncUnitOfWorkExceptionHandlerDecorator{TException}"/> wrapping the given <see cref="IAsyncUnitOfWork"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncUnitOfWork Catch<TException>(this IAsyncUnitOfWork inner, Func<TException, bool> handler)
			where TException : Exception
		{
			return new AsyncUnitOfWorkExceptionHandlerDecorator<TException>(inner, handler);
		}
	}
}
