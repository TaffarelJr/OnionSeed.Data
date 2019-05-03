using System;
using System.Threading.Tasks;
using OnionSeed.Factory;

namespace OnionSeed.Data.Factories
{
	/// <inheritdoc/>
	/// <summary>
	/// Generates random GUID values that can be used as unique identity values for entities.
	/// </summary>
	public class GuidFactory : IFactory<Guid>, IAsyncFactory<Guid>
	{
		/// <inheritdoc/>
		public Guid CreateNew() => Guid.NewGuid();

		/// <inheritdoc/>
		public Task<Guid> CreateNewAsync() => Task.FromResult(Guid.NewGuid());
	}
}
