using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace OnionSeed.Data.Factories
{
	public class SequentialGuidFactoryTests
	{
		private const int TestIterations = 100000;

		[Fact]
		public void Constructor_ShouldThrowException_WhenTypeIsNotRecognized()
		{
			// Act
			Action action = () => new SequentialGuidFactory((SequentialGuidType)10);

			// Assert
			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		public void CreateNew_ShouldReturnNewGuid_WithoutCollisions_WhenTypeIsBinary()
		{
			// Arrange
			var uniqueResults = new HashSet<Guid>();
			var sequentialData = new string[TestIterations];
			var subject = new SequentialGuidFactory(SequentialGuidType.Binary);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				var result = subject.CreateNew();

				// Assert
				uniqueResults.Add(result).Should().BeTrue();
				var sequentialBytes = result.ToByteArray().Take(SequentialGuidFactory.NumberOfSequentialBytes).ToArray();
				sequentialData[i] = BitConverter.ToString(sequentialBytes);
			}

			uniqueResults.Should().HaveCount(TestIterations);
			uniqueResults.Should().NotContain(Guid.Empty);

			sequentialData.Should().BeInAscendingOrder();
		}

		[Fact]
		public void CreateNew_Benchmark_WhenTypeIsBinary()
		{
			// Arrange
			var subject = new SequentialGuidFactory(SequentialGuidType.Binary);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				subject.CreateNew();
			}
		}

		[Fact]
		public void CreateNew_ShouldReturnNewGuid_WithoutCollisions_WhenTypeIsString()
		{
			// Arrange
			const int numberOfChars = (SequentialGuidFactory.NumberOfSequentialBytes * 2) + 1;  // 2 chars per byte, plus one hyphen

			var uniqueResults = new HashSet<Guid>();
			var sequentialData = new string[TestIterations];
			var subject = new SequentialGuidFactory(SequentialGuidType.String);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				var result = subject.CreateNew();

				// Assert
				uniqueResults.Add(result).Should().BeTrue();
				sequentialData[i] = result.ToString().Substring(0, numberOfChars);
			}

			uniqueResults.Should().HaveCount(TestIterations);
			uniqueResults.Should().NotContain(Guid.Empty);

			sequentialData.Should().BeInAscendingOrder();
		}

		[Fact]
		public void CreateNew_Benchmark_WhenTypeIsString()
		{
			// Arrange
			var subject = new SequentialGuidFactory(SequentialGuidType.String);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				subject.CreateNew();
			}
		}

		[Fact]
		public void CreateNew_ShouldReturnNewGuid_WithoutCollisions_WhenTypeIsBinaryEnd()
		{
			// Arrange
			const int numberOfChars = SequentialGuidFactory.NumberOfSequentialBytes * 2;  // 2 chars per byte

			var uniqueResults = new HashSet<Guid>();
			var sequentialData = new string[TestIterations];
			var subject = new SequentialGuidFactory(SequentialGuidType.BinaryEnd);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				var result = subject.CreateNew();

				// Assert
				uniqueResults.Add(result).Should().BeTrue();
				var guidString = result.ToString();
				sequentialData[i] = guidString.Substring(guidString.Length - numberOfChars, numberOfChars);
			}

			uniqueResults.Should().HaveCount(TestIterations);
			uniqueResults.Should().NotContain(Guid.Empty);

			sequentialData.Should().BeInAscendingOrder();
		}

		[Fact]
		public void CreateNew_Benchmark_WhenTypeIsBinaryEnd()
		{
			// Arrange
			var subject = new SequentialGuidFactory(SequentialGuidType.BinaryEnd);

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				subject.CreateNew();
			}
		}
	}
}
