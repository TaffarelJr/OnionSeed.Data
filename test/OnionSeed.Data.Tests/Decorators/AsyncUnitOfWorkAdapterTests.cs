using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	[SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix:Use Async suffix", Justification = "Test methods don't need to end in 'Async'.")]
	public class AsyncUnitOfWorkAdapterTests
	{
		private readonly Mock<IUnitOfWork> _mockInner = new Mock<IUnitOfWork>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new AsyncUnitOfWorkAdapter(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public async Task CommitAsync_ShouldCallSyncMethod()
		{
			// Arrange
			_mockInner.Setup(i => i.Commit());

			var subject = new AsyncUnitOfWorkAdapter(_mockInner.Object);

			// Act
			await subject.CommitAsync().ConfigureAwait(false);

			// Assert
			_mockInner.VerifyAll();
		}
	}
}
