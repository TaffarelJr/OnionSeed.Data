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
	public class AsyncQueryServiceAdapterTests
	{
		private readonly Mock<IQueryService<FakeEntity<int>, int>> _mockInner = new Mock<IQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new AsyncQueryServiceAdapter<FakeEntity<int>, int>(null);

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

			var subject = new AsyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new AsyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new AsyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new AsyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

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

			var subject = new AsyncQueryServiceAdapter<FakeEntity<int>, int>(_mockInner.Object);

			// Act
			var result = await subject.TryGetByIdAsync(entity.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(entity);
			_mockInner.VerifyAll();
		}
	}
}
