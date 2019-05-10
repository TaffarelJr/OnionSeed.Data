using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Wraps a given <see cref="IAsyncRepository{TRoot, TIdentity}"/> and handles any exceptions of the specified type.
	/// </summary>
	/// <typeparam name="TRoot">The type of entities in the data store.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity value of the entities in the data store.</typeparam>
	/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
	public class AsyncRepositoryExceptionHandler<TRoot, TIdentity, TException> : AsyncRepositoryDecorator<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
		where TException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncRepositoryExceptionHandler{TRoot, TIdentity, TException}"/> class,
		/// decorating the given <see cref="IAsyncRepository{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncRepository{TRoot, TIdentity}"/> to be decorated.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public AsyncRepositoryExceptionHandler(IAsyncRepository<TRoot, TIdentity> inner, Func<TException, bool> handler)
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

		/// <inheritdoc/>
		public override async Task AddAsync(TRoot item)
		{
			try
			{
				await Inner.AddAsync(item).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task AddOrUpdateAsync(TRoot item)
		{
			try
			{
				await Inner.AddOrUpdateAsync(item).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task UpdateAsync(TRoot item)
		{
			try
			{
				await Inner.UpdateAsync(item).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TRoot item)
		{
			try
			{
				await Inner.RemoveAsync(item).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task RemoveAsync(TIdentity id)
		{
			try
			{
				await Inner.RemoveAsync(id).ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}

		/// <inheritdoc/>
		public override async Task<bool> TryAddAsync(TRoot item)
		{
			try
			{
				return await Inner.TryAddAsync(item).ConfigureAwait(false);
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
		public override async Task<bool> TryUpdateAsync(TRoot item)
		{
			try
			{
				return await Inner.TryUpdateAsync(item).ConfigureAwait(false);
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
		public override async Task<bool> TryRemoveAsync(TRoot item)
		{
			try
			{
				return await Inner.TryRemoveAsync(item).ConfigureAwait(false);
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
		public override async Task<bool> TryRemoveAsync(TIdentity id)
		{
			try
			{
				return await Inner.TryRemoveAsync(id).ConfigureAwait(false);
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
