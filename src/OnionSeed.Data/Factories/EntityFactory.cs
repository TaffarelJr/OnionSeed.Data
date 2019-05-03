using System;
using OnionSeed.Factory;

namespace OnionSeed.Data.Factories
{
	/// <inheritdoc/>
	/// <summary>
	/// A factory that can construct new entity instances and populate them with new unique identity values.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities created by the factory.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity values.</typeparam>
	public class EntityFactory<TEntity, TIdentity> : IFactory<TEntity>
		where TEntity : IWritableEntity<TIdentity>, new()
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFactory{TEntity, TIdentity}"/> class.
		/// </summary>
		/// <param name="keyGenerator">The factory that will be used to generate unique identity values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="keyGenerator"/> is <b>null</b>.</exception>
		public EntityFactory(IFactory<TIdentity> keyGenerator)
			: this(() => keyGenerator.CreateNew())
		{
			if (keyGenerator == null)
				throw new ArgumentNullException(nameof(keyGenerator));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFactory{TEntity, TIdentity}"/> class.
		/// </summary>
		/// <param name="keyGenerator">The factory method that will be used to generate unique identity values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="keyGenerator"/> is <b>null</b>.</exception>
		public EntityFactory(Func<TIdentity> keyGenerator)
		{
			KeyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
		}

		/// <summary>
		/// Gets the factory method that will be used to generate unique identity values.
		/// </summary>
		protected Func<TIdentity> KeyGenerator { get; }

		/// <inheritdoc />
		public virtual TEntity CreateNew()
		{
			var id = KeyGenerator.Invoke();
			return new TEntity { Id = id };
		}
	}
}
