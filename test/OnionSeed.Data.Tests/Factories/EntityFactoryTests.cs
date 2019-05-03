using System;
using FluentAssertions;
using Moq;
using OnionSeed.Factory;
using Xunit;

namespace OnionSeed.Data.Factories
{
	public class EntityFactoryTests
	{
		[Fact]
		public void Constructor_ShouldThrowException_WhenAbstractFactoryIsNull()
		{
			// Arrange
			IFactory<int> factory = null;

			// Act
			Action action = () => new EntityFactory<FakeEntity<int>, int>(factory);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Constructor_ShouldThrowException_WhenFactoryMethodIsNull()
		{
			// Arrange
			Func<int> factory = null;

			// Act
			Action action = () => new EntityFactory<FakeEntity<int>, int>(factory);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void CreateNew_ShouldCreateEntity_WhenAbstractFactoryIsGiven()
		{
			// Arrange
			const int id = 17;

			var mockFactory = new Mock<IFactory<int>>(MockBehavior.Strict);
			mockFactory
				.Setup(f => f.CreateNew())
				.Returns(id);

			var subject = new EntityFactory<FakeEntity<int>, int>(mockFactory.Object);

			// Act
			var result = subject.CreateNew();

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(id);

			mockFactory.VerifyAll();
		}

		[Fact]
		public void CreateNew_ShouldCreateEntity_WhenSyncFactoryMethodIsGiven()
		{
			// Arrange
			const int id = 17;

			var subject = new EntityFactory<FakeEntity<int>, int>(() => id);

			// Act
			var result = subject.CreateNew();

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(id);
		}
	}
}
