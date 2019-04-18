using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class RepositoryExceptionHandlerDecoratorTests
	{
		private readonly Mock<IRepository<FakeEntity<int>, int>> _mockInner = new Mock<IRepository<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			Action action = () => new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(inner, handler);

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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Add_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 5 };
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Add(entity));

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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

			var subject = new RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>(_mockInner.Object, ex =>
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
