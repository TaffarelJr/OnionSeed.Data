using System;

namespace OnionSeed.Data
{
	public class FakeEntity<TIdentity> : IWritableEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>, IComparable<TIdentity>
	{
		public TIdentity Id { get; set; }

		public string Name { get; set; }
	}
}
