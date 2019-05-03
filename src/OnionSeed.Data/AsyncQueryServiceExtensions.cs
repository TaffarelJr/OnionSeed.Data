using System;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public static class AsyncQueryServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> in a <see cref="AsyncQueryServiceExceptionHandler{TEntity, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncQueryServiceExceptionHandler{TEntity, TIdentity, TException}"/> wrapping the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncQueryService<TEntity, TIdentity> Catch<TEntity, TIdentity, TException>(this IAsyncQueryService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncQueryServiceExceptionHandler<TEntity, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> in a <see cref="SyncQueryServiceAdapter{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="SyncQueryServiceAdapter{TEntity, TIdentity}"/> wrapping the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public static IQueryService<TEntity, TIdentity> ToSync<TEntity, TIdentity>(this IAsyncQueryService<TEntity, TIdentity> inner)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new SyncQueryServiceAdapter<TEntity, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> and <see cref="IAsyncCommandService{TEntity, TIdentity}"/> into a single <see cref="ComposedAsyncRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/>to be joined.</param>
		/// <returns>A new <see cref="ComposedAsyncRepository{TEntity, TIdentity}"/> joining the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> and <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TEntity, TIdentity> Join<TEntity, TIdentity>(this IAsyncQueryService<TEntity, TIdentity> queryService, IAsyncCommandService<TEntity, TIdentity> commandService)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedAsyncRepository<TEntity, TIdentity>(queryService, commandService);
		}
	}
}
