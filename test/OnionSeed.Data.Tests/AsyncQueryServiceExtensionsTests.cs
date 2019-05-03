using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncQueryServiceExtensionsTests
	{
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockQueryService = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockCommandService = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			result.Should().BeOfType<AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockQueryService.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockQueryService.VerifyAll();
		}

		[Fact]
		public void ToSync_ShouldValidateParameter()
		{
			// Arrange
			IAsyncQueryService<FakeEntity<int>, int> inner = null;

			// Act
			Action action = () => inner.ToSync();

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void ToSync_ShouldWrapInner()
		{
			// Act
			var result = _mockQueryService.Object.ToSync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<SyncQueryServiceAdapter<FakeEntity<int>, int>>();

			var adapter = (SyncQueryServiceAdapter<FakeEntity<int>, int>)result;
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
			result.Should().BeOfType<ComposedAsyncRepository<FakeEntity<int>, int>>();

			var composite = (ComposedAsyncRepository<FakeEntity<int>, int>)result;
			composite.QueryService.Should().BeSameAs(_mockQueryService.Object);
			composite.CommandService.Should().BeSameAs(_mockCommandService.Object);

			_mockQueryService.VerifyAll();
			_mockCommandService.VerifyAll();
		}
	}
}
