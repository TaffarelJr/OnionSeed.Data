using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Wraps a given <see cref="IAsyncUnitOfWork"/> and handles any exceptions of the specified type.
	/// </summary>
	/// <typeparam name="TException">"The type of exception to be handled.</typeparam>
	public class AsyncUnitOfWorkExceptionHandler<TException> : AsyncUnitOfWorkDecorator
		where TException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkExceptionHandler{TException}"/> class,
		/// decorating the given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <param name="handler">The handler that will be called when an exception is caught.
		/// This delegate must return a flag indicating if the exception was handled.
		/// If it wasn't, it will be re-thrown after processing.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.
		/// -or- <paramref name="handler"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkExceptionHandler(IAsyncUnitOfWork inner, Func<TException, bool> handler)
			: base(inner)
		{
			Handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		/// <summary>
		/// Gets a reference to the handler that will be called when an exception is caught.
		/// </summary>
		public Func<TException, bool> Handler { get; }

		/// <inheritdoc/>
		public override async Task CommitAsync()
		{
			try
			{
				await Inner.CommitAsync().ConfigureAwait(false);
			}
			catch (TException ex)
			{
				if (!Handler.Invoke(ex))
					throw;
			}
		}
	}
}
