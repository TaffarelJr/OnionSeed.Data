﻿using System;
using OnionSeed.Factory;

namespace OnionSeed.Data.Factories
{
	/// <inheritdoc/>
	/// <summary>
	/// Generates sequential GUID values that can be used as unique identity values for entities.
	/// </summary>
	/// <remarks>The GUIDs generated by this class are sequential, which means they can be used as primary keys in a relational database without incurring
	/// many of the performance hits that would normally come with doing so. The implementation itself encapsulates a strategy by Jeremy Todd on
	/// <a href="http://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database">CodeProject</a>.</remarks>
	public class SequentialGuidFactory : IFactory<Guid>
	{
		/// <summary>
		/// The number of sequential bytes in the GUID.
		/// </summary>
		public const int NumberOfSequentialBytes = 6;

		private const int TotalNumberOfBytes = 16;  // Total number of bytes in the GUID
		private const int NumberOfRandomBytes = TotalNumberOfBytes - NumberOfSequentialBytes;  // Number of random bytes in the GUID
		private const int SequentialOffset = 2;  // Only use the least-significant bytes of the 8-byte time stamp
		private const int NumberOfBytesInData1 = 4;  // Size of Data1 block of GUID
		private const int NumberOfBytesInData2 = 2;  // Size of Data2 block of GUID

		/// <summary>
		/// Initializes a new instance of the <see cref="SequentialGuidFactory"/> class, which will generate keys as specified by <paramref name="sequentialGuidType"/>.
		/// </summary>
		/// <param name="sequentialGuidType">The type of sequential GUID values generated by the current instance.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="sequentialGuidType"/> is not set to a recognized value.</exception>
		public SequentialGuidFactory(SequentialGuidType sequentialGuidType)
		{
			if (!Enum.IsDefined(typeof(SequentialGuidType), sequentialGuidType))
				throw new ArgumentOutOfRangeException(nameof(sequentialGuidType));

			SequentialGuidType = sequentialGuidType;
		}

		/// <summary>
		/// Gets the type of sequential GUID values generated by the current instance.
		/// </summary>
		public SequentialGuidType SequentialGuidType { get; }

		/// <inheritdoc/>
		public Guid CreateNew()
		{
			var randomBytes = GenerateRandomBytes();
			var sequentialBytes = GenerateSequentialBytes();

			var guidBytes = new byte[TotalNumberOfBytes];
			CombineBytes(randomBytes, sequentialBytes, guidBytes);

			return new Guid(guidBytes);
		}

		private static byte[] GenerateRandomBytes()
		{
			return Guid.NewGuid().ToByteArray();
		}

		private static byte[] GenerateSequentialBytes()
		{
			var timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;  // Returns milliseconds (10 ms resolution)
			var sequentialBytes = BitConverter.GetBytes(timestamp);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(sequentialBytes);

			return sequentialBytes;
		}

		private void CombineBytes(byte[] randomBytes, byte[] sequentialBytes, byte[] buffer)
		{
			if (SequentialGuidType == SequentialGuidType.BinaryEnd)
			{
				Buffer.BlockCopy(randomBytes, 0, buffer, 0, NumberOfRandomBytes);
				Buffer.BlockCopy(sequentialBytes, SequentialOffset, buffer, NumberOfRandomBytes, NumberOfSequentialBytes);
			}
			else
			{
				Buffer.BlockCopy(sequentialBytes, SequentialOffset, buffer, 0, NumberOfSequentialBytes);
				Buffer.BlockCopy(randomBytes, 0, buffer, NumberOfSequentialBytes, NumberOfRandomBytes);
			}

			// If formatting as a string, we have to reverse the order of the Data1 and Data2 blocks on little-endian systems
			if (SequentialGuidType == SequentialGuidType.String && BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer, 0, NumberOfBytesInData1);
				Array.Reverse(buffer, NumberOfBytesInData1, NumberOfBytesInData2);
			}
		}
	}
}
