using System;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public static class IAsyncQueryServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> in a <see cref="AsyncQueryServiceExceptionHandlerDecorator{TEntity, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncQueryServiceExceptionHandlerDecorator{TEntity, TIdentity, TException}"/> wrapping the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncQueryService<TEntity, TIdentity> Catch<TEntity, TIdentity, TException>(this IAsyncQueryService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncQueryServiceExceptionHandlerDecorator<TEntity, TIdentity, TException>(inner, handler);
		}
	}
}
