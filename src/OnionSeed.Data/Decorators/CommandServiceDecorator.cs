using System;

namespace OnionSeed.Data.Decorators
{
	/// <inheritdoc/>
	/// <summary>
	/// The base class for decorators for <see cref="ICommandService{TRoot, TIdentity}"/>.
	/// </summary>
	public abstract class CommandServiceDecorator<TRoot, TIdentity> : ICommandService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandServiceDecorator{TRoot, TIdentity}"/> class,
		/// decorating the given <see cref="ICommandService{TRoot, TIdentity}"/>.
		/// </summary>
		/// <param name="inner">The <see cref="ICommandService{TRoot, TIdentity}"/> to be decorated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="inner"/> is <c>null</c>.</exception>
		public CommandServiceDecorator(ICommandService<TRoot, TIdentity> inner)
		{
			Inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		/// <summary>
		/// Gets a reference to the <see cref="ICommandService{TRoot, TIdentity}"/> being decorated.
		/// </summary>
		public ICommandService<TRoot, TIdentity> Inner { get; }

		/// <inheritdoc/>
		public virtual void Add(TRoot item) => Inner.Add(item);

		/// <inheritdoc/>
		public virtual void AddOrUpdate(TRoot item) => Inner.AddOrUpdate(item);

		/// <inheritdoc/>
		public virtual void Update(TRoot item) => Inner.Update(item);

		/// <inheritdoc/>
		public virtual void Remove(TRoot item) => Inner.Remove(item);

		/// <inheritdoc/>
		public virtual void Remove(TIdentity id) => Inner.Remove(id);

		/// <inheritdoc/>
		public virtual bool TryAdd(TRoot item) => Inner.TryAdd(item);

		/// <inheritdoc/>
		public virtual bool TryUpdate(TRoot item) => Inner.TryUpdate(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TRoot item) => Inner.TryRemove(item);

		/// <inheritdoc/>
		public virtual bool TryRemove(TIdentity id) => Inner.TryRemove(id);
	}
}
