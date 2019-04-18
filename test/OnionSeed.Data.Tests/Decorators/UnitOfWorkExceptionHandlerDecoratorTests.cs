using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class UnitOfWorkExceptionHandlerDecoratorTests
	{
		private readonly Mock<IUnitOfWork> _mockInner = new Mock<IUnitOfWork>(MockBehavior.Strict);

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
			Action action = () => new UnitOfWorkExceptionHandlerDecorator<InvalidOperationException>(inner, handler);

			// Arrange
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Commit_ShouldCallInner(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner.Setup(i => i.Commit());

			var subject = new UnitOfWorkExceptionHandlerDecorator<InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			subject.Commit();

			// Assert
			actualException.Should().BeNull();
			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void Commit_ShouldNotCatchException_WhenInnerThrowsException_AndTypeIsWrong(bool handlerReturnValue)
		{
			// Arrange
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Commit())
				.Throws(new Exception());

			var subject = new UnitOfWorkExceptionHandlerDecorator<InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return handlerReturnValue;
			});

			// Act
			Action action = () => subject.Commit();

			// Assert
			action.Should().Throw<Exception>();
			actualException.Should().BeNull();

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldInvokeHandler_AndRethrow_WhenInnerThrowsException_AndHandlerReturnsFalse()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Commit())
				.Throws(expectedException);

			var subject = new UnitOfWorkExceptionHandlerDecorator<InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return false;
			});

			// Act
			Action action = () => subject.Commit();

			// Assert
			action.Should().Throw<InvalidOperationException>();
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldInvokeHandler_WhenInnerThrowsException_AndHandlerReturnsTrue()
		{
			// Arrange
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Commit())
				.Throws(expectedException);

			var subject = new UnitOfWorkExceptionHandlerDecorator<InvalidOperationException>(_mockInner.Object, ex =>
			{
				actualException = ex;
				return true;
			});

			// Act
			subject.Commit();

			// Assert
			actualException.Should().BeSameAs(expectedException);
			_mockInner.VerifyAll();
		}
	}
}
