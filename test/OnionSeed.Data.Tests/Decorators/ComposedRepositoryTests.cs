using System;
using System.Linq;
using FluentAssertions;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class ComposedRepositoryTests
	{
		private readonly Mock<IQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters(bool includeQueryService, bool includeCommandService)
		{
			// Arrange
			var queryService = includeQueryService ? _mockQueryService.Object : null;
			var commandService = includeCommandService ? _mockCommandService.Object : null;

			// Act
			Action action = () => new ComposedRepository<FakeEntity<int>, int>(queryService, commandService);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void GetCount_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			const long count = 38;

			_mockQueryService
				.Setup(i => i.GetCount())
				.Returns(count);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = subject.GetCount();

			// Assert
			result.Should().Be(count);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void GetAll_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			var data = Enumerable.Empty<FakeEntity<int>>();

			_mockQueryService
				.Setup(i => i.GetAll())
				.Returns(data);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = subject.GetAll();

			// Assert
			result.Should().BeSameAs(data);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void GetById_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockQueryService
				.Setup(i => i.GetById(entity.Id))
				.Returns(entity);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = subject.GetById(entity.Id);

			// Assert
			result.Should().BeSameAs(entity);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldCallQueryService_AndReturnFalse_WhenEntityIsNotFound()
		{
			// Arrange
			const int id = 42;
			FakeEntity<int> person = null;

			_mockQueryService
				.Setup(i => i.TryGetById(id, out person))
				.Returns(false);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryGetById(id, out var result);

			// Assert
			success.Should().BeFalse();
			result.Should().BeNull();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldCallQueryService_AndReturnTrue_WhenEntityIsFound()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockQueryService
				.Setup(i => i.TryGetById(entity.Id, out entity))
				.Returns(true);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryGetById(entity.Id, out var result);

			// Assert
			success.Should().BeTrue();
			result.Should().BeSameAs(entity);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Add_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService.Setup(i => i.Add(entity));

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			subject.Add(entity);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService.Setup(i => i.AddOrUpdate(entity));

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			subject.AddOrUpdate(entity);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Update_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService.Setup(i => i.Update(entity));

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			subject.Update(entity);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallCommandService_WhenEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService.Setup(i => i.Remove(entity));

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			subject.Remove(entity);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallCommandService_WhenIdIsGiven()
		{
			// Arrange
			const int id = 42;

			_mockCommandService.Setup(i => i.Remove(id));

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryAdd_ShouldCallCommandService_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryAdd(entity))
				.Returns(expected);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryAdd(entity);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryUpdate_ShouldCallCommandService_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryUpdate(entity))
				.Returns(expected);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryUpdate(entity);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallCommandService_AndReturnResult_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryRemove(entity))
				.Returns(expected);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryRemove(entity);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallCommandService_AndReturnResult_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 42;

			_mockCommandService
				.Setup(i => i.TryRemove(id))
				.Returns(expected);

			var subject = new ComposedRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = subject.TryRemove(id);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
