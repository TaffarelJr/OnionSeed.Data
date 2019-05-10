using System;
using System.Diagnostics.CodeAnalysis;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IQueryService{TRoot, TIdentity}"/>.
	/// </summary>
	public static class QueryServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IQueryService{TRoot, TIdentity}"/> in a <see cref="QueryServiceExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="QueryServiceExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="IQueryService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IQueryService<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this IQueryService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new QueryServiceExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IQueryService{TRoot, TIdentity}"/> in an <see cref="AsyncQueryServiceAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IQueryService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="AsyncQueryServiceAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="IQueryService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		[SuppressMessage("AsyncUsage.CSharp.Naming", "AvoidAsyncSuffix:Avoid Async suffix", Justification = "Name is appropriate in this case.")]
		public static IAsyncQueryService<TRoot, TIdentity> ToAsync<TRoot, TIdentity>(this IQueryService<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncQueryServiceAdapter<TRoot, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IQueryService{TRoot, TIdentity}"/> and <see cref="ICommandService{TRoot, TIdentity}"/> into a single <see cref="ComposedRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="queryService">The <see cref="IQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <param name="commandService">The <see cref="ICommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <returns>A new <see cref="ComposedRepository{TRoot, TIdentity}"/> joining the given <see cref="IQueryService{TRoot, TIdentity}"/> and <see cref="ICommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="queryService"/> is <c>null</c>.
		/// -or- <paramref name="commandService"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> Join<TRoot, TIdentity>(this IQueryService<TRoot, TIdentity> queryService, ICommandService<TRoot, TIdentity> commandService)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedRepository<TRoot, TIdentity>(queryService, commandService);
		}
	}
}
