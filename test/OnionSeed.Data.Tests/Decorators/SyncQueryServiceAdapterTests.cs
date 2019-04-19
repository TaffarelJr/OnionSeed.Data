using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class SyncQueryServiceAdapterTests
	{
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new SyncQueryServiceAdapter<FakeEntity<int>, int>(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void GetCount_ShouldCallAsyncMethod_AndReturnResult()
		{
			// Arrange
			const long count = 38;

			_mockInner
				.Setup(i => i.GetCountAsync())
				.ReturnsAsync(count);

			var subject = new SyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = subject.GetCount();

			// Assert
			result.Should().Be(count);
			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetAll_ShouldCallAsyncMethod_AndReturnResult()
		{
			// Arrange
			var data = Enumerable.Empty<FakeEntity<int>>();

			_mockInner
				.Setup(i => i.GetAllAsync())
				.ReturnsAsync(data);

			var subject = new SyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = subject.GetAll();

			// Assert
			result.Should().BeSameAs(data);
			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetById_ShouldCallAsyncMethod_AndReturnResult()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.GetByIdAsync(entity.Id))
				.ReturnsAsync(entity);

			var subject = new SyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = subject.GetById(entity.Id);

			// Assert
			result.Should().BeSameAs(entity);
			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldCallAsyncMethod_AndReturnFalse_WhenEntityIsNotFound()
		{
			// Arrange
			const int id = 42;

			_mockInner
				.Setup(i => i.TryGetByIdAsync(id))
				.ReturnsAsync((FakeEntity<int>)null);

			var subject = new SyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryGetById(id, out FakeEntity<int> result);

			// Assert
			success.Should().BeFalse();
			result.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldCallAsyncMethod_AndReturnTrue_WhenEntityIsFound()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryGetByIdAsync(entity.Id))
				.ReturnsAsync(entity);

			var subject = new SyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryGetById(entity.Id, out FakeEntity<int> result);

			// Assert
			success.Should().BeTrue();
			result.Should().BeSameAs(entity);

			_mockInner.VerifyAll();
		}
	}
}
