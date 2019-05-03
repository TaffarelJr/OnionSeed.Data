using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	public class CommandServiceExtensionsTests
	{
		private readonly Mock<IQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockTap = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
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
			result.Should().BeOfType<CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (CommandServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockCommandService.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockCommandService.VerifyAll();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void WithTap_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => inner.WithTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithTap_ShouldWrapInner()
		{
			// Act
			var result = _mockCommandService.Object.WithTap(_mockTap.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<CommandServiceTap<FakeEntity<int>, int>>();

			var decorator = (CommandServiceTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeNull();
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<CommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (CommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
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
		public void WithTap_ShouldValidateParameters_WhenLoggerIsGiven(bool includeInner, bool includeTap, bool includeLogger)
		{
			// Arrange
			var inner = includeInner ? _mockCommandService.Object : null;
			var tap = includeTap ? _mockTap.Object : null;
			var logger = includeLogger ? _mockLogger : null;

			// Act
			Action action = () => inner.WithTap(tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void WithTap_ShouldWrapInner_AndLogger_WhenLoggerIsGiven()
		{
			// Act
			var result = _mockCommandService.Object.WithTap(_mockTap.Object, _mockLogger.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<CommandServiceTap<FakeEntity<int>, int>>();

			var decorator = (CommandServiceTap<FakeEntity<int>, int>)result;
			decorator.Inner.Should().BeSameAs(_mockCommandService.Object);
			decorator.Logger.Should().BeSameAs(_mockLogger.Object);
			decorator.Tap.Should().NotBeNull();
			decorator.Tap.Should().BeOfType<CommandServiceExceptionHandler<FakeEntity<int>, int, Exception>>();

			var exDecorator = (CommandServiceExceptionHandler<FakeEntity<int>, int, Exception>)decorator.Tap;
			exDecorator.Inner.Should().BeSameAs(_mockTap.Object);

			_mockCommandService.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void ToAsync_ShouldValidateParameter()
		{
			// Arrange
			ICommandService<FakeEntity<int>, int> inner = null;

			// Act
			Action action = () => inner.ToAsync();

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void ToAsync_ShouldWrapInner()
		{
			// Act
			var result = _mockCommandService.Object.ToAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceAdapter<FakeEntity<int>, int>>();

			var adapter = (AsyncCommandServiceAdapter<FakeEntity<int>, int>)result;
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
			Action action = () => commandService.Join(queryService);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}

		[Fact]
		public void Join_ShouldWrapServices()
		{
			// Act
			var result = _mockCommandService.Object.Join(_mockQueryService.Object);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ComposedRepository<FakeEntity<int>, int>>();

			var composite = (ComposedRepository<FakeEntity<int>, int>)result;
			composite.QueryService.Should().BeSameAs(_mockQueryService.Object);
			composite.CommandService.Should().BeSameAs(_mockCommandService.Object);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
