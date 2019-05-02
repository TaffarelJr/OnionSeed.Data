using System;
using OnionSeed.Helpers.Async;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IAsyncUnitOfWork"/> to work like an <see cref="IUnitOfWork"/>.
	/// </summary>
	public class SyncUnitOfWorkAdapter : AsyncUnitOfWorkDecorator, IUnitOfWork
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncUnitOfWorkAdapter"/> class,
		/// wrapping the given <see cref="IAsyncUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IAsyncUnitOfWork"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public SyncUnitOfWorkAdapter(IAsyncUnitOfWork inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public void Commit() => AsyncExtensions.RunSynchronously(() => Inner.CommitAsync());
	}
}
