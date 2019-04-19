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
	public class IAsyncUnitOfWorkExtensionsTests
	{
		private readonly Mock<IAsyncUnitOfWork> _mockInner = new Mock<IAsyncUnitOfWork>(MockBehavior.Strict);

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
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.CommitAsync())
				.ThrowsAsync(expectedException);

			// Act
			var result = _mockInner.Object.Catch((InvalidOperationException ex) =>
			{
				actualException = ex;
				return true;
			});

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<AsyncUnitOfWorkExceptionHandlerDecorator<InvalidOperationException>>();

			await result.CommitAsync().ConfigureAwait(false);
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
