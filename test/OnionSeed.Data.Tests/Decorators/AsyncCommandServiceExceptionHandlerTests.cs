using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncCommandServiceExceptionHandlerTests
	{
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			Action action = () => new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(inner, handler);

			// Arrange
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task AddAsync_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void AddAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task AddOrUpdateAsync_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void AddOrUpdateAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdateAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task UpdateAsync_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.UpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void UpdateAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.UpdateAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.UpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.UpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task RemoveAsync_ShouldCallInner_WhenEntityIsGiven(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void RemoveAsync_ShouldNotCatchException_WhenEntityIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldInvokeHandler_AndRethrow_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldInvokeHandler_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task RemoveAsync_ShouldCallInner_WhenIdIsGiven(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void RemoveAsync_ShouldNotCatchException_WhenIdIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(id))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldInvokeHandler_AndRethrow_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldInvokeHandler_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.RemoveAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryAddAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryAddAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAddAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAddAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAddAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryUpdateAsync_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryUpdateAsync_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdateAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdateAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndReturnResult_WhenEntityIsGiven(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemoveAsync_ShouldNotCatchException_WhenEntityIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(entity))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldInvokeHandler_AndRethrow_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldInvokeHandler_AndReturnResult_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(entity))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndReturnResult_WhenIdIsGiven(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.Returns(Task.FromResult(expectedResult));

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemoveAsync_ShouldNotCatchException_WhenIdIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.ThrowsAsync(new Exception());

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldInvokeHandler_AndRethrow_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldInvokeHandler_AndReturnResult_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.ThrowsAsync(expectedException);

			var subject = new AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
