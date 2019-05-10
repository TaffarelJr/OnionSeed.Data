using System;

namespace OnionSeed.Data
{
	/// <inheritdoc />
	/// <summary>
	/// Defines a mechanism that can be used to asynchronously store entities in a data store
	/// and query them back out again.
	/// </summary>
	public interface IAsyncRepository<TRoot, in TIdentity> : IAsyncQueryService<TRoot, TIdentity>, IAsyncCommandService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
	}
}
