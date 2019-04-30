using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	public class RepositoryExtensionsTests
	{
		private readonly Mock<IRepository<FakeEntity<int>, int>> _mockInner = new Mock<IRepository<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IRepository<FakeEntity<int>, int>> _mockTap = new Mock<IRepository<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Catch_ShouldValidateParameters(bool includeInner, bool includeHandler)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var handler = includeHandler ? (InvalidOperationException ex) => false : (Func<InvalidOperationException, bool>)null;

			// Act
			Action action = () => inner.Catch(handler);

			// Assert
			action.Should().Throw<ArgumentNullException>();
			_mockInner.VerifyAll();
		}

		[Fact]
		public void Catch_ShouldWrapInner()
		{
			// Arrange
			Func<InvalidOperationException, bool> handler = (InvalidOperationException ex) => false;

			// Act
			var result = _mockInner.Object.Catch(handler);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockInner.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockInner.VerifyAll();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void WithTap_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => inner.WithTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithTap_ShouldWrapInner()
		{
			// Act
			var result = _mockInner.Object.WithTap(_mockTap.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<RepositoryTapDecorator<FakeEntity<int>, int>>();

			var decorator = (RepositoryTapDecorator<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockInner.Object);
			decorator.Logger.Should().BeNull();
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, Exception>>();

			var exDecorator = (RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false, false, false)]
		[InlineData(false, false, true)]
		[InlineData(false, true, false)]
		[InlineData(false, true, true)]
		[InlineData(true, false, false)]
		[InlineData(true, false, true)]
		public void WithTap_ShouldValidateParameters_WhenLoggerIsGiven(bool includeInner, bool includeTap, bool includeLogger)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var tap = includeTap ? _mockTap.Object : null;
			var logger = includeLogger ? _mockLogger : null;

			// Act
			Action action = () => inner.WithTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithTap_ShouldWrapInner_AndLogger_WhenLoggerIsGiven()
		{
			// Act
			var result = _mockInner.Object.WithTap(_mockTap.Object, _mockLogger.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<RepositoryTapDecorator<FakeEntity<int>, int>>();

			var decorator = (RepositoryTapDecorator<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockInner.Object);
			decorator.Logger.Should().BeSameAs(_mockLogger.Object);
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, Exception>>();

			var exDecorator = (RepositoryExceptionHandlerDecorator<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}
	}
}
