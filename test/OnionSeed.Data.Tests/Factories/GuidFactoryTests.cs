using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace OnionSeed.Data.Factories
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Tests don't need to end in 'Async'.")]
	public class GuidFactoryTests
	{
		private const int TestIterations = 100000;

		[Fact]
		public void CreateNew_ShouldReturnNewGuid_WithoutCollisions()
		{
			// Arrange
			var uniqueResults = new HashSet<Guid>();
			var subject = new GuidFactory();

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				var result = subject.CreateNew();

				// Assert
				uniqueResults.Add(result).Should().BeTrue();
			}

			uniqueResults.Should().HaveCount(TestIterations);
			uniqueResults.Should().NotContain(Guid.Empty);
		}

		[Fact]
		public void CreateNew_Benchmark()
		{
			// Arrange
			var subject = new GuidFactory();

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				subject.CreateNew();
			}
		}

		[Fact]
		public async Task CreateNewAsync_ShouldReturnNewGuid_WithoutCollisions()
		{
			// Arrange
			var tasks = new Task<Guid>[TestIterations];
			var subject = new GuidFactory();

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				tasks[i] = subject.CreateNewAsync();
			}

			// Assert
			var results = await Task.WhenAll(tasks).ConfigureAwait(false);

			var uniqueResults = new HashSet<Guid>(results);
			uniqueResults.Should().HaveCount(TestIterations);
			uniqueResults.Should().NotContain(Guid.Empty);
		}

		[Fact]
		public async Task CreateNewAsync_Benchmark()
		{
			// Arrange
			var tasks = new Task<Guid>[TestIterations];
			var subject = new GuidFactory();

			for (var i = 0; i < TestIterations; i++)
			{
				// Act
				tasks[i] = subject.CreateNewAsync();
			}

			// Assert
			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
	}
}
