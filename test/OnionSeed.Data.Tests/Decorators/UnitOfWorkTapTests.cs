using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class UnitOfWorkTapTests
	{
		private readonly Mock<IUnitOfWork> _mockInner = new Mock<IUnitOfWork>(MockBehavior.Strict);
		private readonly Mock<IUnitOfWork> _mockTap = new Mock<IUnitOfWork>(MockBehavior.Strict);
		private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => new UnitOfWorkTap(inner, tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldCallInner_AndTap()
		{
			// Arrange
			_mockInner.Setup(u => u.Commit());
			_mockTap.Setup(u => u.Commit());

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Commit();

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.Commit())
				.Throws(new InvalidOperationException());

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.Commit();

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			_mockInner.Setup(u => u.Commit());
			_mockTap
				.Setup(u => u.Commit())
				.Throws(new InvalidOperationException());

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Commit();

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			_mockInner.Setup(u => u.Commit());
			_mockTap.Setup(u => u.Commit());

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Commit();

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.Commit())
				.Throws(new InvalidOperationException());

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.Commit();

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Commit_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			_mockInner.Setup(u => u.Commit());
			_mockTap
				.Setup(u => u.Commit())
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new UnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Commit();

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}
	}
}
