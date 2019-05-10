using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="IRepository{TRoot, TIdentity}"/>.
	/// </summary>
	public static class RepositoryExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="IRepository{TRoot, TIdentity}"/> in a <see cref="RepositoryExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="RepositoryExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="IRepository{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this IRepository<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new RepositoryExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="IRepository{TRoot, TIdentity}"/> in a <see cref="RepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="RepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> WithTap<TRoot, TIdentity>(this IRepository<TRoot, TIdentity> inner, IRepository<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new RepositoryTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="IRepository{TRoot, TIdentity}"/> in a <see cref="RepositoryTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="IRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="IRepository{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="RepositoryTap{TRoot, TIdentity}"/> wrapping the two given <see cref="IRepository{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> WithTap<TRoot, TIdentity>(this IRepository<TRoot, TIdentity> inner, IRepository<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new RepositoryTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="IRepository{TRoot, TIdentity}"/> in an <see cref="AsyncRepositoryAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="IRepository{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="AsyncRepositoryAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="IRepository{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		[SuppressMessage("AsyncUsage.CSharp.Naming", "AvoidAsyncSuffix:Avoid Async suffix", Justification = "Name is appropriate in this case.")]
		public static IAsyncRepository<TRoot, TIdentity> ToAsync<TRoot, TIdentity>(this IRepository<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncRepositoryAdapter<TRoot, TIdentity>(inner);
		}
	}
}
