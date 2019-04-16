using System;
using OnionSeed.Types;

namespace OnionSeed
{
	public class FakeEntity<TKey> : IWritableEntity<TKey>
		where TKey : IEquatable<TKey>, IComparable<TKey>
	{
		public TKey Id { get; set; }

		public string Name { get; set; }
	}
}
