using System;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Wraps a given <see cref="ICommandService{TRoot, TIdentity}"/> and handles any exceptions of the specified type.
	/// </summary>
	/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
	public class CommandServiceExceptionHandler<TRoot, TIdentity, TException> : CommandServiceDecorator<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		where TException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandServiceExceptionHandler{TRoot, TIdentity, TException}"/> class,
		/// decorating the given <see cref="ICommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public CommandServiceExceptionHandler(ICommandService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			: base(inner)
		{
			Handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		/// <summary>
		/// Gets a reference to the handler that will be called when an exception is caught.
		/// </summary>
		public Func<TException, bool> Handler { get; }

		/// <inheritdoc/>
		public override void Add(TRoot item)
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
		public override void AddOrUpdate(TRoot item)
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
		public override void Update(TRoot item)
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
		public override void Remove(TRoot item)
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
		public override bool TryAdd(TRoot item)
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
		public override bool TryUpdate(TRoot item)
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
		public override bool TryRemove(TRoot item)
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
