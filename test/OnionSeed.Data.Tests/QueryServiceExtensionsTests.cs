using System;
using FluentAssertions;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	public class QueryServiceExtensionsTests
	{
		private readonly Mock<IQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Catch_ShouldValidateParameters(bool includeInner, bool includeHandler)
		{
			// Arrange
			var inner = includeInner ? _mockQueryService.Object : null;
			var handler = includeHandler ? (InvalidOperationException ex) => false : (Func<InvalidOperationException, bool>)null;

			// Act
			Action action = () => inner.Catch(handler);

			// Assert
			action.Should().Throw<ArgumentNullException>();
			_mockQueryService.VerifyAll();
		}

		[Fact]
		public void Catch_ShouldWrapInner()
		{
			// Arrange
			Func<InvalidOperationException, bool> handler = (InvalidOperationException ex) => false;

			// Act
			var result = _mockQueryService.Object.Catch(handler);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<QueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (QueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockQueryService.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockQueryService.VerifyAll();
		}

		[Fact]
		public void ToAsync_ShouldValidateParameter()
		{
			// Arrange
			IQueryService<FakeEntity<int>, int> inner = null;

			// Act
			Action action = () => inner.ToAsync();

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void ToAsync_ShouldWrapInner()
		{
			// Act
			var result = _mockQueryService.Object.ToAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncQueryServiceAdapter<FakeEntity<int>, int>>();

			var adapter = (AsyncQueryServiceAdapter<FakeEntity<int>, int>)result;
			adapter.Inner.Should().BeSameAs(_mockQueryService.Object);

			_mockQueryService.VerifyAll();
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
			result.Should().BeOfType<ComposedRepository<FakeEntity<int>, int>>();

			var composite = (ComposedRepository<FakeEntity<int>, int>)result;
			composite.QueryService.Should().BeSameAs(_mockQueryService.Object);
			composite.CommandService.Should().BeSameAs(_mockCommandService.Object);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
