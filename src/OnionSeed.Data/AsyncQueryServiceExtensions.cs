using System;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public static class AsyncQueryServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> in a <see cref="AsyncQueryServiceExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncQueryServiceExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncQueryService<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this IAsyncQueryService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncQueryServiceExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> in a <see cref="SyncQueryServiceAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="SyncQueryServiceAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public static IQueryService<TRoot, TIdentity> ToSync<TRoot, TIdentity>(this IAsyncQueryService<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new SyncQueryServiceAdapter<TRoot, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and <see cref="IAsyncCommandService{TRoot, TIdentity}"/> into a single <see cref="ComposedAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <returns>A new <see cref="ComposedAsyncRepository{TRoot, TIdentity}"/> joining the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> Join<TRoot, TIdentity>(this IAsyncQueryService<TRoot, TIdentity> queryService, IAsyncCommandService<TRoot, TIdentity> commandService)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedAsyncRepository<TRoot, TIdentity>(queryService, commandService);
		}
	}
}
