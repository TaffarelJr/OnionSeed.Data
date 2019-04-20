using System;
using FluentAssertions;
using Moq;
using OnionSeed.Data.Decorators;
using Xunit;

namespace OnionSeed.Data
{
	public class ICommandServiceExtensionsTests
	{
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockInner = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);

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
			const int id = 5;
			var expectedException = new InvalidOperationException();
			InvalidOperationException actualException = null;

			_mockInner
				.Setup(i => i.Remove(id))
				.Throws(expectedException);

			// Act
			var result = _mockInner.Object.Catch((InvalidOperationException ex) =>
			{
				actualException = ex;
				return true;
			});

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<CommandServiceExceptionHandlerDecorator<FakeEntity<int>, int, InvalidOperationException>>();

			result.Remove(id);
			actualException.Should().BeSameAs(expectedException);

			_mockInner.VerifyAll();
		}
	}
}
