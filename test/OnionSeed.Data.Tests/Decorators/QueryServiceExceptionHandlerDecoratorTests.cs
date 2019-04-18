using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class QueryServiceExceptionHandlerDecoratorTests
	{
		private readonly Mock<IQueryService<FakeEntity<int>, int>> _mockInner = new Mock<IQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			Action action = () => new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(inner, handler);

			// Arrange
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetCount_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const long expectedResult = 123;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCount())
				.Returns(expectedResult);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.GetCount();

			// Assert
			actualResult.Should().Be(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetCount_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCount())
				.Throws(new Exception());

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.GetCount();

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetCount_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCount())
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.GetCount();

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetCount_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetCount())
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.GetCount();

			// Assert
			actualResult.Should().Be(default);
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetAll_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
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
				.Setup(i => i.GetAll())
				.Returns(expectedResult);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.GetAll();

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetAll_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAll())
				.Throws(new Exception());

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.GetAll();

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetAll_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAll())
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.GetAll();

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetAll_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetAll())
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.GetAll();

			// Assert
			actualResult.Should().NotBeNull();
			actualResult.Should().BeEquivalentTo(Enumerable.Empty<FakeEntity<int>>());
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetById_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedResult = new FakeEntity<int>() { Id = id };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetById(id))
				.Returns(expectedResult);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualResult = subject.GetById(id);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetById_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetById(id))
				.Throws(new Exception());

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.GetById(id);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetById_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetById(id))
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.GetById(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void GetById_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.GetById(id))
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualResult = subject.GetById(id);

			// Assert
			actualResult.Should().BeNull();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryGetById_ShouldCallInner_AndReturnResult(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			var expectedFlag = true;
			var expectedResult = new FakeEntity<int>() { Id = id };
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetById(id, out expectedResult))
				.Returns(expectedFlag);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			var actualFlag = subject.TryGetById(id, out var actualResult);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
			actualFlag.Should().Be(expectedFlag);
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryGetById_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			const int id = 5;
			FakeEntity<int> dummyResult = null;
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetById(id, out dummyResult))
				.Throws(new Exception());

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.TryGetById(id, out dummyResult);

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			const int id = 5;
			FakeEntity<int> dummyResult = null;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetById(id, out dummyResult))
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.TryGetById(id, out dummyResult);

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void TryGetById_ShouldInvokeHandler_AndReturnResult_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			const int id = 5;
			var dummyResult = new FakeEntity<int>() { Id = id };
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryGetById(id, out dummyResult))
				.Throws(expectedException);

			var subject = new QueryServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			var actualFlag = subject.TryGetById(id, out var actualResult);

			// Assert
			actualResult.Should().BeNull();
			actualFlag.Should().BeFalse();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
