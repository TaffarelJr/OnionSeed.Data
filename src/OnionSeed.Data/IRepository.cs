using System;

namespace OnionSeed.Data
{
	/// <inheritdoc />
	/// <summary>
	/// Defines a mechanism that can be used to store entities in a data store
	/// and query them back out again.
	/// </summary>
	public interface IRepository<TEntity, in TIdentity> : IQueryService<TEntity, TIdentity>, ICommandService<TEntity, TIdentity>
		where TEntity : IAggregateRoot<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
	}
}
