using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class CommandServiceExceptionHandlerTests
	{
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockInner = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			Action action = () => new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(inner, handler);

			// Arrange
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Add_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Add(entity));

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.Add(entity);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Add_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Add(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.Add(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Add_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Add(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.Add(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Add_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Add(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.Add(entity);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void AddOrUpdate_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.AddOrUpdate(entity));

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.AddOrUpdate(entity);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void AddOrUpdate_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdate(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.AddOrUpdate(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdate(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.AddOrUpdate(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.AddOrUpdate(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.AddOrUpdate(entity);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Update_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Update(entity));

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.Update(entity);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Update_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Update(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.Update(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Update_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Update(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.Update(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Update_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Update(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.Update(entity);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Remove_ShouldCallInner_WhenEntityIsGiven(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Remove(entity));

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.Remove(entity);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Remove_ShouldNotCatchException_WhenEntityIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.Remove(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldInvokeHandler_AndRethrow_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.Remove(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldInvokeHandler_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.Remove(entity);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Remove_ShouldCallInner_WhenIdIsGiven(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Remove(id));

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.Remove(id);

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Remove_ShouldNotCatchException_WhenIdIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(id))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.Remove(id);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldInvokeHandler_AndRethrow_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(id))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.Remove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldInvokeHandler_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(id))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.Remove(id);

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryAdd_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAdd(entity))
				.Returns(expectedResult);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.TryAdd(entity);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryAdd_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAdd(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.TryAdd(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAdd(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.TryAdd(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryAdd(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.TryAdd(entity);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryUpdate_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdate(entity))
				.Returns(expectedResult);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.TryUpdate(entity);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryUpdate_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdate(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.TryUpdate(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdate(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.TryUpdate(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryUpdate(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.TryUpdate(entity);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndReturnResult_WhenEntityIsGiven(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(entity))
				.Returns(expectedResult);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.TryRemove(entity);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldNotCatchException_WhenEntityIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(entity))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.TryRemove(entity);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldInvokeHandler_AndRethrow_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.TryRemove(entity);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldInvokeHandler_AndReturnResult_WhenEntityIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(entity))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.TryRemove(entity);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndReturnResult_WhenIdIsGiven(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedResult = true;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(id))
				.Returns(expectedResult);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.TryRemove(id);

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldNotCatchException_WhenIdIsGiven_AndInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(id))
				.Throws(new Exception());

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.TryRemove(id);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldInvokeHandler_AndRethrow_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(id))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.TryRemove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldInvokeHandler_AndReturnResult_WhenIdIsGiven_AndInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemove(id))
				.Throws(expectedException);

			var subject = new CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.TryRemove(id);

			// Assert
			actualResult.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
