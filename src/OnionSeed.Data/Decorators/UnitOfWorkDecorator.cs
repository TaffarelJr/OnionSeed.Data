using System;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="IUnitOfWork"/>.
	/// </summary>
	public abstract class UnitOfWorkDecorator : IUnitOfWork
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkDecorator"/> class,
		/// decorating the given <see cref="IUnitOfWork"/>.
		/// </summary>
		/// <param name="inner">The <see cref="IUnitOfWork"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public UnitOfWorkDecorator(IUnitOfWork inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="IUnitOfWork"/> being decorated.
		/// </summary>
		protected IUnitOfWork Inner { get; }

		/// <inheritdoc/>
		public virtual void Commit() => Inner.Commit();
	}
}
