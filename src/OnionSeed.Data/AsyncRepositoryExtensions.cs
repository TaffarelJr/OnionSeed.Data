using System;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public static class AsyncRepositoryExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a <see cref="AsyncRepositoryExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="AsyncRepositoryExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this IAsyncRepository<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new AsyncRepositoryExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a sequential <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> WithSequentialTap<TRoot, TIdentity>(this IAsyncRepository<TRoot, TIdentity> inner, IAsyncRepository<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncRepositoryTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a sequential <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> WithSequentialTap<TRoot, TIdentity>(this IAsyncRepository<TRoot, TIdentity> inner, IAsyncRepository<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncRepositoryTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a parallel <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> WithParallelTap<TRoot, TIdentity>(this IAsyncRepository<TRoot, TIdentity> inner, IAsyncRepository<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncRepositoryParallelTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a parallel <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IAsyncRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="AsyncRepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IAsyncRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IAsyncRepository<TRoot, TIdentity> WithParallelTap<TRoot, TIdentity>(this IAsyncRepository<TRoot, TIdentity> inner, IAsyncRepository<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncRepositoryParallelTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IAsyncRepository{TRoot, TIdentity}"/> in a <see cref="SyncRepositoryAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="SyncRepositoryAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> ToSync<TRoot, TIdentity>(this IAsyncRepository<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new SyncRepositoryAdapter<TRoot, TIdentity>(inner);
		}
	}
}
