using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OnionSeed.Factory;
using Xunit;

namespace OnionSeed.Data.Factories
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Tests don't need to end in 'Async'.")]
	public class AsyncEntityFactoryTests
	{
		[Fact]
		public void Constructor_ShouldThrowException_WhenAbstractFactoryIsNull()
		{
			// Arrange
			IAsyncFactory<int> factory = null;

			// Act
			Action action = () => new AsyncEntityFactory<FakeEntity<int>, int>(factory);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Constructor_ShouldThrowException_WhenFactoryMethodIsNull()
		{
			// Arrange
			Func<Task<int>> factory = null;

			// Act
			Action action = () => new AsyncEntityFactory<FakeEntity<int>, int>(factory);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public async Task CreateNewAsync_ShouldCreateEntity_WhenAbstractFactoryIsGiven()
		{
			// Arrange
			const int id = 17;

			var mockFactory = new Mock<IAsyncFactory<int>>(MockBehavior.Strict);
			mockFactory
				.Setup(f => f.CreateNewAsync())
				.ReturnsAsync(id);

			var subject = new AsyncEntityFactory<FakeEntity<int>, int>(mockFactory.Object);

			// Act
			var result = await subject.CreateNewAsync().ConfigureAwait(false);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(id);

			mockFactory.VerifyAll();
		}

		[Fact]
		public async Task CreateNewAsync_ShouldCreateEntity_WhenFactoryMethodIsGiven()
		{
			// Arrange
			const int id = 17;

			var subject = new AsyncEntityFactory<FakeEntity<int>, int>(() => Task.FromResult(id));

			// Act
			var result = await subject.CreateNewAsync().ConfigureAwait(false);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(id);
		}
	}
}
