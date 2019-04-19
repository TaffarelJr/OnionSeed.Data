using System;
using FluentAssertions;
using Moq;
using OnionSeed.Helpers.Async;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class SyncUnitOfWorkAdapterTests
	{
		private readonly Mock<IAsyncUnitOfWork> _mockInner = new Mock<IAsyncUnitOfWork>(MockBehavior.Strict);

		[Fact]
		public void Constructor_ShouldThrowException_WhenInnerIsNull()
		{
			// Act
			Action action = () => new SyncUnitOfWorkAdapter(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Commit_ShouldCallAsyncMethod()
		{
			// Arrange
			_mockInner
				.Setup(i => i.CommitAsync())
				.Returns(TaskHelpers.CompletedTask);

			var subject = new SyncUnitOfWorkAdapter(_mockInner.Object);

			// Act
			subject.Commit();

			// Assert
			_mockInner.VerifyAll();
		}
	}
}
