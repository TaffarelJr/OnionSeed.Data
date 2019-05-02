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
		private readonly Mock<IAsyncQueryService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncQueryService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			result.Should().BeOfType<AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>>();

			var typedResult = (AsyncQueryServiceExceptionHandler<FakeEntity<int>, int, InvalidOperationException>)result;
			typedResult.Inner.Should().BeSameAs(_mockInner.Object);
			typedResult.Handler.Should().BeSameAs(handler);

			_mockInner.VerifyAll();
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
			var result = _mockInner.Object.ToSync();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<SyncQueryServiceAdapter<FakeEntity<int>, int>>();

			var adapter = (SyncQueryServiceAdapter<FakeEntity<int>, int>)result;
			adapter.Inner.Should().BeSameAs(_mockInner.Object);

			_mockInner.VerifyAll();
		}
	}
}
