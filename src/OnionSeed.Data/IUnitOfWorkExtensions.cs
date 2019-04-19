using System;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IUnitOfWork"/>.
	/// </summary>
	public static class IUnitOfWorkExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IUnitOfWork"/> in a <see cref="UnitOfWorkExceptionHandlerDecorator{TException}"/>.
		/// </summary>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="UnitOfWorkExceptionHandlerDecorator{TException}"/> wrapping the given <see cref="IUnitOfWork"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IUnitOfWork Catch<TException>(this IUnitOfWork inner, Func<TException, bool> handler)
			where TException : Exception
		{
			return new UnitOfWorkExceptionHandlerDecorator<TException>(inner, handler);
		}
	}
}
