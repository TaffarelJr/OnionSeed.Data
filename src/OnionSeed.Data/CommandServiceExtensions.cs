using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="ICommandService{TRoot, TIdentity}"/>.
	/// </summary>
	public static class CommandServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="ICommandService{TRoot, TIdentity}"/> in a <see cref="CommandServiceExceptionHandler{TRoot, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="CommandServiceExceptionHandler{TRoot, TIdentity, TException}"/> wrapping the given <see cref="ICommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static ICommandService<TRoot, TIdentity> Catch<TRoot, TIdentity, TException>(this ICommandService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new CommandServiceExceptionHandler<TRoot, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="ICommandService{TRoot, TIdentity}"/> in a <see cref="CommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="ICommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="ICommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="CommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="ICommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static ICommandService<TRoot, TIdentity> WithTap<TRoot, TIdentity>(this ICommandService<TRoot, TIdentity> inner, ICommandService<TRoot, TIdentity> tap)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new CommandServiceTap<TRoot, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="ICommandService{TRoot, TIdentity}"/> in a <see cref="CommandServiceTap{TRoot, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="ICommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="ICommandService{TRoot, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="CommandServiceTap{TRoot, TIdentity}"/> wrapping the two given <see cref="ICommandService{TRoot, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static ICommandService<TRoot, TIdentity> WithTap<TRoot, TIdentity>(this ICommandService<TRoot, TIdentity> inner, ICommandService<TRoot, TIdentity> tap, ILogger logger)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new CommandServiceTap<TRoot, TIdentity>(inner, tap, logger);
		}

		/// <summary>
		/// Wraps the given <see cref="ICommandService{TRoot, TIdentity}"/> in an <see cref="AsyncCommandServiceAdapter{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be wrapped.</param>
		/// <returns>A new <see cref="AsyncCommandServiceAdapter{TRoot, TIdentity}"/> wrapping the given <see cref="ICommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		[SuppressMessage("AsyncUsage.CSharp.Naming", "AvoidAsyncSuffix:Avoid Async suffix", Justification = "Name is appropriate in this case.")]
		public static IAsyncCommandService<TRoot, TIdentity> ToAsync<TRoot, TIdentity>(this ICommandService<TRoot, TIdentity> inner)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new AsyncCommandServiceAdapter<TRoot, TIdentity>(inner);
		}

		/// <summary>
		/// Joins the given <see cref="IQueryService{TRoot, TIdentity}"/> and <see cref="ICommandService{TRoot, TIdentity}"/> into a single <see cref="ComposedRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <param name="commandService">The <see cref="ICommandService{TRoot, TIdentity}"/>to be joined.</param>
		/// <param name="queryService">The <see cref="IQueryService{TRoot, TIdentity}"/> to be joined.</param>
		/// <returns>A new <see cref="ComposedRepository{TRoot, TIdentity}"/> joining the given <see cref="IQueryService{TRoot, TIdentity}"/> and <see cref="ICommandService{TRoot, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="commandService"/> is <c>null</c>.
		/// -or- <paramref name="queryService"/> is <c>null</c>.</exception>
		public static IRepository<TRoot, TIdentity> Join<TRoot, TIdentity>(this ICommandService<TRoot, TIdentity> commandService, IQueryService<TRoot, TIdentity> queryService)
			where TRoot : IAggregateRoot<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new ComposedRepository<TRoot, TIdentity>(queryService, commandService);
		}
	}
}
