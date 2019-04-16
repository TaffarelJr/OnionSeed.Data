using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IAsyncUnitOfWork"/>.
	/// </summary>
	public abstract class AsyncUnitOfWorkDecorator : IAsyncUnitOfWork
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkDecorator"/> class,
		/// decorating the given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkDecorator(IAsyncUnitOfWork inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IAsyncUnitOfWork"/> being decorated.
		/// </summary>
		protected IAsyncUnitOfWork Inner { get; }

		/// <inheritdoc/>
		public virtual Task CommitAsync() => Inner.CommitAsync();
	}
}
