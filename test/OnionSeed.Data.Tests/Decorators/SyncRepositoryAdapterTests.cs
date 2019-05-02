using System;
using System.Linq;
using FluentAssertions;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class SyncRepositoryAdapterTests
	{
		private readonly Mock<IAsyncRepository<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncRepository<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new SyncRepositoryAdapter<FakeEntity<int>, int>(null);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryGetById(entity.Id, out FakeEntity<int> result);

			// Assert
			success.Should().BeTrue();
			result.Should().BeSameAs(entity);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Add_ShouldCallAsyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new SyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = subject.TryRemove(id);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}
	}
}
