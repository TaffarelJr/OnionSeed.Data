using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncCommandServiceExtensionsTests
	{
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockTap = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Catch_ShouldValidateParameters(bool includeInner, bool includeHandler)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var handler = includeHandler ? (InvalidOperationException ex) => false : (Func<InvalidOperationException, bool>)null;

			// Act
			Action action = () => inner.Catch(handler);

			// Assert
			action.Should().Throw<ArgumentNullException>();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Catch_ShouldWrapInner()
		{
			// Arrange
			Func<InvalidOperationException, bool> handler = (InvalidOperationException ex) => false;

			// Act
			var result = _mockCommandService.Object.Catch(handler);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockCommandService.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void WithSequentialTap_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => inner.WithSequentialTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithSequentialTap_ShouldWrapInner()
		{
			// Act
			var result = _mockCommandService.Object.WithSequentialTap(_mockTap.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceTap<FakeEntity<int>, int>>();

			var decorator = (AsyncCommandServiceTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeNull();
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockCommandService.VerifyAll();
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
		public void WithSequentialTap_ShouldValidateParameters_WhenLoggerIsGiven(bool includeInner, bool includeTap, bool includeLogger)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;
			var logger = includeLogger ? _mockLogger : null;

			// Act
			Action action = () => inner.WithSequentialTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithSequentialTap_ShouldWrapInner_AndLogger_WhenLoggerIsGiven()
		{
			// Act
			var result = _mockCommandService.Object.WithSequentialTap(_mockTap.Object, _mockLogger.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceTap<FakeEntity<int>, int>>();

			var decorator = (AsyncCommandServiceTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeSameAs(_mockLogger.Object);
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void WithParallelTap_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => inner.WithParallelTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithParallelTap_ShouldWrapInner()
		{
			// Act
			var result = _mockCommandService.Object.WithParallelTap(_mockTap.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceParallelTap<FakeEntity<int>, int>>();

			var decorator = (AsyncCommandServiceParallelTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeNull();
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockCommandService.VerifyAll();
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
		public void WithParallelTap_ShouldValidateParameters_WhenLoggerIsGiven(bool includeInner, bool includeTap, bool includeLogger)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;
			var logger = includeLogger ? _mockLogger : null;

			// Act
			Action action = () => inner.WithParallelTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithParallelTap_ShouldWrapInner_AndLogger_WhenLoggerIsGiven()
		{
			// Act
			var result = _mockCommandService.Object.WithParallelTap(_mockTap.Object, _mockLogger.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceParallelTap<FakeEntity<int>, int>>();

			var decorator = (AsyncCommandServiceParallelTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeSameAs(_mockLogger.Object);
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (AsyncCommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void ToSync_ShouldValidateParameter()
		{
			// Arrange
			IAsyncCommandService<FakeEntity<int>, int> inner = null;

			// Act
			Action action = () => inner.ToSync();

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void ToSync_ShouldWrapInner()
		{
			// Act
			var result = _mockCommandService.Object.ToSync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<SyncCommandServiceAdapter<FakeEntity<int>, int>>();

			var adapter = (SyncCommandServiceAdapter<FakeEntity<int>, int>)result;
			adapter.Inner.Should().BeSameAs(_mockCommandService.Object);

			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Join_ShouldValidateParameters(bool includeQueryService, bool includeCommandService)
		{
			// Arrange
			var queryService = includeQueryService ? _mockQueryService.Object : null;
			var commandService = includeCommandService ? _mockCommandService.Object : null;

			// Act
			Action action = () => queryService.Join(commandService);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Join_ShouldWrapServices()
		{
			// Act
			var result = _mockQueryService.Object.Join(_mockCommandService.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ComposedAsyncRepository<FakeEntity<int>, int>>();

			var composite = (ComposedAsyncRepository<FakeEntity<int>, int>)result;
			composite.QueryService.Should().BeSameAs(_mockQueryService.Object);
			composite.CommandService.Should().BeSameAs(_mockCommandService.Object);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
