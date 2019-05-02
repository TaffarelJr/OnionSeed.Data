using System;
using System.Threading.Tasks;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// Adapts an <see cref="IUnitOfWork"/> to work like an <see cref="IAsyncUnitOfWork"/>.
	/// </summary>
	public class AsyncUnitOfWorkAdapter : UnitOfWorkDecorator, IAsyncUnitOfWork
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncUnitOfWorkAdapter"/> class,
		/// wrapping the given <see cref="IUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be wrapped.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public AsyncUnitOfWorkAdapter(IUnitOfWork inner)
			: base(inner)
		{
		}

		/// <inheritdoc/>
		public Task CommitAsync() => Task.Run(() => Inner.Commit());
	}
}
