using System;
using FluentAssertions;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class SyncCommandServiceAdapterTests
	{
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new SyncCommandServiceAdapter<FakeEntity<int>, int>(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Add_ShouldCallAsyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			subject.Add(entity);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldCallAsyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.AddOrUpdateAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			subject.AddOrUpdate(entity);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public void Update_ShouldCallAsyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.UpdateAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			subject.Update(entity);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallAsyncMethod_WhenEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.RemoveAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			subject.Remove(entity);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallAsyncMethod_WhenIdIsGiven()
		{
			// Arrange
			const int id = 42;

			_mockInner
				.Setup(i => i.RemoveAsync(id))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryAdd_ShouldCallAsyncMethod_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryAddAsync(entity))
				.ReturnsAsync(expected);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryAdd(entity);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryUpdate_ShouldCallAsyncMethod_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryUpdateAsync(entity))
				.ReturnsAsync(expected);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryUpdate(entity);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallAsyncMethod_AndReturnResult_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryRemoveAsync(entity))
				.ReturnsAsync(expected);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryRemove(entity);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallAsyncMethod_AndReturnResult_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 42;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.ReturnsAsync(expected);

			var subject = new SyncCommandServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryRemove(id);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}
	}
}
