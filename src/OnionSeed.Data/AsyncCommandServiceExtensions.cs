using System;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
	/// </summary>
	public static class AsyncCommandServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a <see cref="AsyncCommandServiceExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncCommandServiceExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this IAsyncCommandService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncCommandServiceExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a sequential <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TRoot, TIdentity> WithSequentialTap<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a sequential <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TRoot, TIdentity> WithSequentialTap<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a parallel <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TRoot, TIdentity> WithParallelTap<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceParallelTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a parallel <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncCommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncCommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncCommandService<TRoot, TIdentity> WithParallelTap<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> inner, IAsyncCommandService<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceParallelTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/> in a <see cref="SyncCommandServiceAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="SyncCommandServiceAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public static ICommandService<TRoot, TIdentity> ToSync<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new SyncCommandServiceAdapter<TRoot, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and <see cref="IAsyncCommandService{TRoot, TIdentity}"/> into a single <see cref="ComposedAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="commandService">The <see cref="IAsyncCommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <param name="queryService">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <returns>A new <see cref="ComposedAsyncRepository{TRoot, TIdentity}"/> joining the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and <see cref="IAsyncCommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="commandService"/> is <c>null</c>.
		/// -or- <paramref name="queryService"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> Join<TRoot, TIdentity>(this IAsyncCommandService<TRoot, TIdentity> commandService, IAsyncQueryService<TRoot, TIdentity> queryService)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedAsyncRepository<TRoot, TIdentity>(queryService, commandService);
		}
	}
}
