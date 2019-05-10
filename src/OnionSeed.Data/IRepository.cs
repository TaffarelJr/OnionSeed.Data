using System;

namespace OnionSeed.Data
{
	/// <inheritdoc />
	/// <summary>
	/// Defines a mechanism that can be used to store entities in a data store
	/// and query them back out again.
	/// </summary>
	public interface IRepository<TRoot, in TIdentity> : IQueryService<TRoot, TIdentity>, ICommandService<TRoot, TIdentity>
		where TRoot : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
	}
}
