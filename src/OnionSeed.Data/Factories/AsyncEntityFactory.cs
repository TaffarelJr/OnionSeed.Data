using System;
using System.Threading.Tasks;
using OnionSeed.Factory;

namespace OnionSeed.Data.Factories
{
	/// <inheritdoc/>
	/// <summary>
	/// A factory that can construct new entity instances and populate them with new unique identity values.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities created by the factory.</typeparam>
	/// <typeparam name="TIdentity">The type of the unique identity values.</typeparam>
	public class AsyncEntityFactory<TEntity, TIdentity> : IAsyncFactory<TEntity>
		where TEntity : IWritableEntity<TIdentity>, new()
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncEntityFactory{TEntity, TIdentity}"/> class.
		/// </summary>
		/// <param name="keyGenerator">The factory that will be used to generate unique identity values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="keyGenerator"/> is <b>null</b>.</exception>
		public AsyncEntityFactory(IAsyncFactory<TIdentity> keyGenerator)
			: this(() => keyGenerator.CreateNewAsync())
		{
			if (keyGenerator == null)
				throw new ArgumentNullException(nameof(keyGenerator));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncEntityFactory{TEntity, TIdentity}"/> class.
		/// </summary>
		/// <param name="keyGenerator">The factory method that will be used to generate unique identity values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="keyGenerator"/> is <c>null</c>.</exception>
		public AsyncEntityFactory(Func<Task<TIdentity>> keyGenerator)
		{
			KeyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
		}

		/// <summary>
		/// Gets the factory method that will be used to generate unique identity values.
		/// </summary>
		protected Func<Task<TIdentity>> KeyGenerator { get; }

		/// <inheritdoc />
		public virtual async Task<TEntity> CreateNewAsync()
		{
			var id = await KeyGenerator.Invoke().ConfigureAwait(false);
			return new TEntity { Id = id };
		}
	}
}
