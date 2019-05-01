using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncQueryServiceExceptionHandlerTests
	{
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters(bool includeInner, bool includeHandler)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var handler = includeHandler ? (InvalidOperationException ex) => false : (Func<InvalidOperationException, bool>)null;

			// Act
			Action action = () => new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(inner, handler);

			// Arrange
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task GetCountAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const long expectedResult = 123;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCountAsync())
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetCountAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCountAsync())
				.ThrowsAsync(new Exception());

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetCountAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCountAsync())
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task GetCountAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCountAsync())
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(default);
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task GetAllAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			var expectedResult = new[]
			{
				new FakeEntity<int>() { Id = 1 },
				new FakeEntity<int>() { Id = 2 },
				new FakeEntity<int>() { Id = 3 }
			};
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAllAsync())
				.Returns(Task.FromResult<IEnumerable<FakeEntity<int>>>(expectedResult));

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetAllAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAllAsync())
				.ThrowsAsync(new Exception());

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetAllAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAllAsync())
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task GetAllAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAllAsync())
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.GetAllAsync().ConfigureAwait(false);

			// Assert
			actualResult.Should().NotBeNull();
			actualResult.Should().BeEquivalentTo(Enumerable.Empty<FakeEntity<int>>());
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task GetByIdAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedResult = new FakeEntity<int>() { Id = id };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetByIdAsync(id))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.GetByIdAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetByIdAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetByIdAsync(id))
				.ThrowsAsync(new Exception());

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.GetByIdAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetByIdAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetByIdAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.GetByIdAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetByIdAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.GetByIdAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeNull();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryGetByIdAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedResult = new FakeEntity<int>() { Id = id };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetByIdAsync(id))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryGetByIdAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetByIdAsync(id))
				.ThrowsAsync(new Exception());

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryGetByIdAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetByIdAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetByIdAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.TryGetByIdAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeNull();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
