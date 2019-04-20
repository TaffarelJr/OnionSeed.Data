using System;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Wraps a given <see cref="ICommandService{TEntity, TIdentity}"/> and handles any exceptions of the specified type.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
	public class CommandServiceExceptionHandlerDecorator<TEntity, TIdentity, TException> : CommandServiceDecorator<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		where TException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandServiceExceptionHandlerDecorator{TEntity, TIdentity, TException}"/> class,
		/// decorating the given <see cref="ICommandService{TEntity, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="ICommandService{TEntity, TIdentity}"/> to be decorated.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public CommandServiceExceptionHandlerDecorator(ICommandService<TEntity, TIdentity> inner, Func<TException, bool> handler)
			: base(inner)
		{
			Handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		/// <summary>
		/// Gets a reference to the handler that will be called when an exception is caught.
		/// </summary>
		protected Func<TException, bool> Handler { get; }

		/// <inheritdoc/>
		public override void Add(TEntity item)
		{
			try
			{
				Inner.Add(item);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override void AddOrUpdate(TEntity item)
		{
			try
			{
				Inner.AddOrUpdate(item);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override void Update(TEntity item)
		{
			try
			{
				Inner.Update(item);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override void Remove(TEntity item)
		{
			try
			{
				Inner.Remove(item);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override void Remove(TIdentity id)
		{
			try
			{
				Inner.Remove(id);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override bool TryAdd(TEntity item)
		{
			try
			{
				return Inner.TryAdd(item);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return false;
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override bool TryUpdate(TEntity item)
		{
			try
			{
				return Inner.TryUpdate(item);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return false;
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override bool TryRemove(TEntity item)
		{
			try
			{
				return Inner.TryRemove(item);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return false;
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override bool TryRemove(TIdentity id)
		{
			try
			{
				return Inner.TryRemove(id);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return false;
				else
					throw;
			}
		}
	}
}
