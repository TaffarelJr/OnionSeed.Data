using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncUnitOfWorkTapDecoratorTests
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
			Action action = () => new AsyncUnitOfWorkTapDecorator(inner, tap);

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
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object);

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

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object);

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
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object);

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
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

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

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

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
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.CommitAsync())
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.CommitAsync())
				.Returns(Task.FromResult(0));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task CommitAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			_mockInner
				.Setup(u => u.CommitAsync())
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void CommitAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			_mockInner
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.CommitAsync())
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncUnitOfWorkTapDecorator(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}
	}
}
