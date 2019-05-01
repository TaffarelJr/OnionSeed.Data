using System;
using Microsoft.Extensions.Logging;
using OnionSeed.Data.Decorators;

namespace OnionSeed.Data
{
	/// <summary>
	/// Contains extension methods for <see cref="ICommandService{TEntity, TIdentity}"/>.
	/// </summary>
	public static class CommandServiceExtensions
	{
		/// <summary>
		/// Wraps the given <see cref="ICommandService{TEntity, TIdentity}"/> in a <see cref="CommandServiceExceptionHandlerDecorator{TEntity, TIdentity, TException}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
		/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <returns>A new <see cref="CommandServiceExceptionHandlerDecorator{TEntity, TIdentity, TException}"/> wrapping the given <see cref="ICommandService{TEntity, TIdentity}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public static ICommandService<TEntity, TIdentity> Catch<TEntity, TIdentity, TException>(this ICommandService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
			where TException : Exception
		{
			return new CommandServiceExceptionHandlerDecorator<TEntity, TIdentity, TException>(inner, handler);
		}

		/// <summary>
		/// Wraps the given <see cref="ICommandService{TEntity, TIdentity}"/> in a <see cref="CommandServiceTapDecorator{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="ICommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="ICommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <returns>A new <see cref="CommandServiceTapDecorator{TEntity, TIdentity}"/> wrapping the two given <see cref="ICommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static ICommandService<TEntity, TIdentity> WithTap<TEntity, TIdentity>(this ICommandService<TEntity, TIdentity> inner, ICommandService<TEntity, TIdentity> tap)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new CommandServiceTapDecorator<TEntity, TIdentity>(inner, tap);
		}

		/// <summary>
		/// Wraps the given <see cref="ICommandService{TEntity, TIdentity}"/> in a <see cref="CommandServiceTapDecorator{TEntity, TIdentity}"/>,
		/// mirroring all commands to the other given <see cref="ICommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
		/// <typeparam name="TIdentity">The type of the unique identity value of the entitites in the data store.</typeparam>
		/// <param name="inner">The <see cref="ICommandService{TEntity, TIdentity}"/> to be wrapped.</param>
		/// <param name="tap">The tap <see cref="ICommandService{TEntity, TIdentity}"/>, where commands will be mirrored.</param>
		/// <param name="logger">The <see cref="ILogger"/> to which any tap exceptions should be written.</param>
		/// <returns>A new <see cref="CommandServiceTapDecorator{TEntity, TIdentity}"/> wrapping the two given <see cref="ICommandService{TEntity, TIdentity}"/> instances.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="tap"/> is <c>null</c>.</exception>
		public static ICommandService<TEntity, TIdentity> WithTap<TEntity, TIdentity>(this ICommandService<TEntity, TIdentity> inner, ICommandService<TEntity, TIdentity> tap, ILogger logger)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		{
			return new CommandServiceTapDecorator<TEntity, TIdentity>(inner, tap, logger);
		}
	}
}
