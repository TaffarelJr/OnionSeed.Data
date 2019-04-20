using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class IAsyncCommandServiceExtensionsTests
	{
		private readonly Mock<IAsyncCommandService<FakeEntity<int>, int>> _mockInner = new Mock<IAsyncCommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
		public async Task Catch_ShouldWrapInner()
		{
			// Arrange
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.TryRemoveAsync(id))
				.ThrowsAsync(expectedException);

			// Act
			var result = _mockInner.Object.Catch((InvalidOperationException ex) =>
			{
				actualException = ex;
				return true;
			});

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncCommandServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>>();

			var testResult = await result.TryRemoveAsync(id).ConfigureAwait(false);
			testResult.Should().Be(default);
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
