using System;
using System.Diagnostics.CodeAnalysis;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IQueryService{TEntity, TIdentity}"/>.
	/// </summary>
	public static class QueryServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IQueryService{TEntity, TIdentity}"/> in a <see cref="QueryServiceExceptionHandler{TEntity, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="QueryServiceExceptionHandler{TEntity, TIdentity, TException}"/> wrapping the given <see cref="IQueryService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IQueryService<TEntity, TIdentity> Catch<TEntity, TIdentity, TException>(this IQueryService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new QueryServiceExceptionHandler<TEntity, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IQueryService{TEntity, TIdentity}"/> in an <see cref="AsyncQueryServiceAdapter{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IQueryService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="AsyncQueryServiceAdapter{TEntity, TIdentity}"/> wrapping the given <see cref="IQueryService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		[SuppressMessage("AsyncUsage.CSharp.Naming", "AvoidAsyncSuffix:Avoid Async suffix", Justification = "Name is appropriate in this case.")]
		public static IAsyncQueryService<TEntity, TIdentity> ToAsync<TEntity, TIdentity>(this IQueryService<TEntity, TIdentity> inner)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncQueryServiceAdapter<TEntity, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IQueryService{TEntity, TIdentity}"/> and <see cref="ICommandService{TEntity, TIdentity}"/> into a single <see cref="ComposedRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="queryService">The <see cref="IQueryService{TEntity, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="ICommandService{TEntity, TIdentity}"/>to be joined.</param>
		/// <returns>A new <see cref="ComposedRepository{TEntity, TIdentity}"/> joining the given <see cref="IQueryService{TEntity, TIdentity}"/> and <see cref="ICommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public static IRepository<TEntity, TIdentity> Join<TEntity, TIdentity>(this IQueryService<TEntity, TIdentity> queryService, ICommandService<TEntity, TIdentity> commandService)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedRepository<TEntity, TIdentity>(queryService, commandService);
		}
	}
}
