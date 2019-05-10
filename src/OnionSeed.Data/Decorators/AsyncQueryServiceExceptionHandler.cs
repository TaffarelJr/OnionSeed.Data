using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Wraps a given <see cref="IAsyncQueryService{TRoot, TIdentity}"/> and handles any exceptions of the specified type.
	/// </summary>
	/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
	public class AsyncQueryServiceExceptionHandler<TRoot, TIdentity, TException> : AsyncQueryServiceDecorator<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		where TException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncQueryServiceExceptionHandler{TRoot, TIdentity, TException}"/> class,
		/// decorating the given <see cref="IAsyncQueryService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncQueryService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public AsyncQueryServiceExceptionHandler(IAsyncQueryService<TRoot, TIdentity> inner, Func<TException, bool> handler)
			: base(inner)
		{
			Handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		/// <summary>
		/// Gets a reference to the handler that will be called when an exception is caught.
		/// </summary>
		public Func<TException, bool> Handler { get; }

		/// <inheritdoc/>
		public override async Task<long> GetCountAsync()
		{
			try
			{
				return await Inner.GetCountAsync().ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return 0;
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task<IEnumerable<TRoot>> GetAllAsync()
		{
			try
			{
				return await Inner.GetAllAsync().ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return Enumerable.Empty<TRoot>();
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task<TRoot> GetByIdAsync(TIdentity id)
		{
			try
			{
				return await Inner.GetByIdAsync(id).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return default;
				else
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task<TRoot> TryGetByIdAsync(TIdentity id)
		{
			try
			{
				return await Inner.TryGetByIdAsync(id).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (Handler.Invoke(ex))
					return default;
				else
					throw;
			}
		}
	}
}
