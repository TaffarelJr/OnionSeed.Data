using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class ComposedAsyncRepositoryTests
	{
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			Action action = () => new ComposedAsyncRepository<FakeEntity<int>, int>(queryService, commandService);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task GetCountAsync_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			const long count = 38;

			_mockQueryService
				.Setup(i => i.GetCountAsync())
				.ReturnsAsync(count);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			result.Should().Be(count);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task GetAllAsync_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			var data = Enumerable.Empty<FakeEntity<int>>();

			_mockQueryService
				.Setup(i => i.GetAllAsync())
				.ReturnsAsync(data);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(data);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldCallQueryService_AndReturnResult()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockQueryService
				.Setup(i => i.GetByIdAsync(entity.Id))
				.ReturnsAsync(entity);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = await subject.GetByIdAsync(entity.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(entity);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldCallQueryService_AndReturnNull_WhenEntityIsNotFound()
		{
			// Arrange
			const int id = 42;

			_mockQueryService
				.Setup(i => i.TryGetByIdAsync(id))
				.ReturnsAsync((FakeEntity<int>)null);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			result.Should().BeNull();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldCallQueryService_AndReturnEntity_WhenEntityIsFound()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockQueryService
				.Setup(i => i.TryGetByIdAsync(entity.Id))
				.ReturnsAsync(entity);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var result = await subject.TryGetByIdAsync(entity.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(entity);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.AddAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.AddOrUpdateAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallCommandService()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.UpdateAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallCommandService_WhenEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.RemoveAsync(entity))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallCommandService_WhenIdIsGiven()
		{
			// Arrange
			const int id = 42;

			_mockCommandService
				.Setup(i => i.RemoveAsync(id))
				.Returns(TaskHelpers.CompletedTask);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryAddAsync_ShouldCallCommandService_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryAddAsync(entity))
				.ReturnsAsync(expected);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryUpdateAsync_ShouldCallCommandService_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryUpdateAsync(entity))
				.ReturnsAsync(expected);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallCommandService_AndReturnResult_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockCommandService
				.Setup(i => i.TryRemoveAsync(entity))
				.ReturnsAsync(expected);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallCommandService_AndReturnResult_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 42;

			_mockCommandService
				.Setup(i => i.TryRemoveAsync(id))
				.ReturnsAsync(expected);

			var subject = new ComposedAsyncRepository<FakeEntity<int>, int>(_mockQueryService.Object, _mockCommandService.Object);

			// Act
			var success = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
