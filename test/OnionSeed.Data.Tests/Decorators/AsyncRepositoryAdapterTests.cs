using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncRepositoryAdapterTests
	{
		private readonly Mock<IRepository<FakeEntity<int>, int>> _mockInner = new Mock<IRepository<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new AsyncRepositoryAdapter<FakeEntity<int>, int>(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public async Task GetCountAsync_ShouldCallSyncMethod_AndReturnResult()
		{
			// Arrange
			const long count = 38;

			_mockInner
				.Setup(i => i.GetCount())
				.Returns(count);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			result.Should().Be(count);
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task GetAllAsync_ShouldCallSyncMethod_AndReturnResult()
		{
			// Arrange
			var data = Enumerable.Empty<FakeEntity<int>>();

			_mockInner
				.Setup(i => i.GetAll())
				.Returns(data);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(data);
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldCallSyncMethod_AndReturnResult()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.GetById(entity.Id))
				.Returns(entity);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.GetByIdAsync(entity.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(entity);
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldCallSyncMethod_AndReturnNull_WhenEntityIsNotFound()
		{
			// Arrange
			const int id = 42;
			FakeEntity<int> person = null;

			_mockInner
				.Setup(i => i.TryGetById(id, out person))
				.Returns(false);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			result.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldCallSyncMethod_AndReturnEntity_WhenEntityIsFound()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryGetById(entity.Id, out entity))
				.Returns(true);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.TryGetByIdAsync(entity.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(entity);
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallSyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner.Setup(i => i.Add(entity));

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallSyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner.Setup(i => i.AddOrUpdate(entity));

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallSyncMethod()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner.Setup(i => i.Update(entity));

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallSyncMethod_WhenEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner.Setup(i => i.Remove(entity));

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallSyncMethod_WhenIdIsGiven()
		{
			// Arrange
			const int id = 42;

			_mockInner.Setup(i => i.Remove(id));

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryAddAsync_ShouldCallSyncMethod_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryAdd(entity))
				.Returns(expected);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryUpdateAsync_ShouldCallSyncMethod_AndReturnResult(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryUpdate(entity))
				.Returns(expected);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallSyncMethod_AndReturnResult_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 42 };

			_mockInner
				.Setup(i => i.TryRemove(entity))
				.Returns(expected);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallSyncMethod_AndReturnResult_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 42;

			_mockInner
				.Setup(i => i.TryRemove(id))
				.Returns(expected);

			var subject = new AsyncRepositoryAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var success = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			success.Should().Be(expected);
			_mockInner.VerifyAll();
		}
	}
}
