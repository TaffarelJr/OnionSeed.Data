using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncUnitOfWorkTapTests
	{
		private readonly Mock<IAsyncUnitOfWork> _mockInner = new Mock<IAsyncUnitOfWork>(MockBehavior.Strict);
		private readonly Mock<IAsyncUnitOfWork> _mockTap = new Mock<IAsyncUnitOfWork>(MockBehavior.Strict);
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
			Action action = () => new AsyncUnitOfWorkTap(inner, tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncUnitOfWorkTap(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}
	}
}
