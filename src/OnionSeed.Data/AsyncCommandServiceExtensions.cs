using System;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
	/// </summary>
	public static class AsyncCommandServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a <see cref="AsyncCommandServiceExceptionHandler{TEntity, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncCommandServiceExceptionHandler{TEntity, TIdentity, TException}"/> wrapping the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TEntity, TIdentity> Catch<TEntity, TIdentity, TException>(this IAsyncCommandService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncCommandServiceExceptionHandler<TEntity, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a sequential <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TEntity, TIdentity> WithSequentialTap<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceTap<TEntity, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a sequential <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TEntity, TIdentity> WithSequentialTap<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, ILogger logger)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceTap<TEntity, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a parallel <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TEntity, TIdentity> WithParallelTap<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceParallelTap<TEntity, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a parallel <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TEntity, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TEntity, TIdentity> WithParallelTap<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> inner, IAsyncCommandService<TEntity, TIdentity> tap, ILogger logger)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceParallelTap<TEntity, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/> in a <see cref="SyncCommandServiceAdapter{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="SyncCommandServiceAdapter{TEntity, TIdentity}"/> wrapping the given <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public static ICommandService<TEntity, TIdentity> ToSync<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> inner)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new SyncCommandServiceAdapter<TEntity, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> and <see cref="IAsyncCommandService{TEntity, TIdentity}"/> into a single <see cref="ComposedAsyncRepository{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TEntity, TIdentity}"/>to be joined.</param>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TEntity, TIdentity}"/> to be joined.</param>
		/// <returns>A new <see cref="ComposedAsyncRepository{TEntity, TIdentity}"/> joining the given <see cref="IAsyncQueryService{TEntity, TIdentity}"/> and <see cref="IAsyncCommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="commandService"/> is <c>null</c>.
		/// -or- <paramref name="queryService"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TEntity, TIdentity> Join<TEntity, TIdentity>(this IAsyncCommandService<TEntity, TIdentity> commandService, IAsyncQueryService<TEntity, TIdentity> queryService)
			where TEntity : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedAsyncRepository<TEntity, TIdentity>(queryService, commandService);
		}
	}
}
