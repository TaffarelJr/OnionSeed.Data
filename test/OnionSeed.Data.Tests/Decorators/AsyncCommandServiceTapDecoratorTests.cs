﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncCommandServiceTapDecoratorTests
	{
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockTap = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
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
			Action action = () => new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(inner, tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.AddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task UpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.UpdateAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.UpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldSkipTap_WhenEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldSkipTap_WhenLoggerIsGiven_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenParallel_AndEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenParallel_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenParallel_AndEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenParallel_AndEntityIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.RemoveAsync(entity))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldSkipTap_WhenIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldSkipTap_WhenLoggerIsGiven_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenParallel_AndIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenParallel_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenParallel_AndIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenParallel_AndIdIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel_AndIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task RemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.RemoveAsync(id))
				.Returns(Task.FromResult(0));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryAddAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryAddAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryAddAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldCallInner_AndTap()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldCallInner_AndTap_WhenParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_WhenParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldDoNothing_WhenParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_WhenParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndTapThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };
			var expectedResult = true;

			_mockInner
				.Setup(u => u.TryUpdateAsync(entity))
				.Returns(Task.FromResult(expectedResult));
			_mockTap
				.Setup(u => u.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expectedResult);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryUpdateAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.AddOrUpdateAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldSkipTap_WhenEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldSkipTap_WhenLoggerIsGiven_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenParallel_AndEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenParallel_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenParallel_AndEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenParallel_AndEntityIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemoveAsync(entity))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndEntityIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			var entity = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(r => r.TryRemoveAsync(entity))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(entity))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(entity).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldSkipTap_WhenIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldSkipTap_WhenLoggerIsGiven_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenParallel_AndIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenParallel_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenParallel_AndIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenParallel_AndIdIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, parallelMode: true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndParallel_AndIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.Returns(Task.FromResult(0));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task TryRemoveAsync_ShouldLogException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemoveAsync(id))
				.Returns(Task.FromResult(expected));
			_mockTap
				.Setup(u => u.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			var actualResult = await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			actualResult.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_AndLogException_WhenLoggerIsGiven_AndParallel_AndIdIsGiven_AndBothThrowExceptions()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(r => r.TryRemoveAsync(id))
				.ThrowsAsync(new IndexOutOfRangeException());
			_mockTap
				.Setup(r => r.RemoveAsync(id))
				.ThrowsAsync(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new AsyncCommandServiceTapDecorator<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object, true);

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(id).ConfigureAwait(false);

			// Assert
			action.Should().Throw<IndexOutOfRangeException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockTap.VerifyAll();
		}
	}
}
