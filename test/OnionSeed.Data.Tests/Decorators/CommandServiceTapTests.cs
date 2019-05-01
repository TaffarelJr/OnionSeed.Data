using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OnionSeed.Data.Decorators
{
	public class CommandServiceTapTests
	{
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockInner = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ICommandService<FakeEntity<int>, int>> _mockTap = new Mock<ICommandService<FakeEntity<int>, int>>(MockBehavior.Strict);
		private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>(MockBehavior.Strict);

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters(bool includeInner, bool includeTap)
		{
			// Arrange
			var inner = includeInner ? _mockInner.Object : null;
			var tap = includeTap ? _mockTap.Object : null;

			// Act
			Action action = () => new CommandServiceTap<FakeEntity<int>, int>(inner, tap);

			// Assert
			action.Should().Throw<ArgumentNullException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldCallInner_AndTap()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Add(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Add(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Add(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.Add(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Add(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Add(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Add(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Add(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Add(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.Add(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Add_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Add(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Add(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldCallInner_AndTap()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.AddOrUpdate(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.AddOrUpdate(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.AddOrUpdate(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.AddOrUpdate(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.AddOrUpdate(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.AddOrUpdate(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.AddOrUpdate(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.AddOrUpdate(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void AddOrUpdate_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.AddOrUpdate(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.AddOrUpdate(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldCallInner_AndTap()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Update(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Update(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Update(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.Update(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Update(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Update(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Update(person));
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Update(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Update(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.Update(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Update_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Update(person));
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Update(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallInner_AndTap_WhenEntityIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Remove(person));
			_mockTap.Setup(u => u.Remove(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Remove(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldSkipTap_WhenEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.Remove(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldDoNothing_WhenEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Remove(person));
			_mockTap
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Remove(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndEntityIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Remove(person));
			_mockTap.Setup(u => u.Remove(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Remove(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldSkipTap_WhenLoggerIsGiven_AndEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.Remove(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldLogException_WhenLoggerIsGiven_AndEntityIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner.Setup(u => u.Remove(person));
			_mockTap
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Remove(person);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallInner_AndTap_WhenIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner.Setup(u => u.Remove(id));
			_mockTap.Setup(u => u.Remove(id));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldSkipTap_WhenIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.Remove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldDoNothing_WhenIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner.Setup(u => u.Remove(id));
			_mockTap
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndIdIsGiven()
		{
			// Arrange
			const int id = 7;

			_mockInner.Setup(u => u.Remove(id));
			_mockTap.Setup(u => u.Remove(id));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldSkipTap_WhenLoggerIsGiven_AndIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.Remove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void Remove_ShouldLogException_WhenLoggerIsGiven_AndIdIsGiven_AndTapThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner.Setup(u => u.Remove(id));
			_mockTap
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			subject.Remove(id);

			// Assert
			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldCallInner_AndTap()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(true);
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldSkipTap_WhenInnerReturnsFalse()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(false);

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeFalse();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.TryAdd(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(true);
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(true);
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldSkipTap_WhenLoggerIsGiven_AndInnerReturnsFalse()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(false);

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeFalse();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.TryAdd(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryAdd_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryAdd(person))
				.Returns(true);
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryAdd(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldCallInner_AndTap()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(true);
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldSkipTap_WhenInnerReturnsFalse()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(false);

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeFalse();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldSkipTap_WhenInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.TryUpdate(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldDoNothing_WhenTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(true);
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldCallInner_AndTap_WhenLoggerIsGiven()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(true);
			_mockTap.Setup(u => u.AddOrUpdate(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldSkipTap_WhenLoggerIsGiven_AndInnerReturnsFalse()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(false);

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeFalse();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldSkipTap_WhenLoggerIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.TryUpdate(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryUpdate_ShouldLogException_WhenLoggerIsGiven_AndTapThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryUpdate(person))
				.Returns(true);
			_mockTap
				.Setup(u => u.AddOrUpdate(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryUpdate(person);

			// Assert
			result.Should().BeTrue();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndTap_WhenEntityIsGiven(bool expected)
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Returns(expected);
			_mockTap.Setup(u => u.Remove(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryRemove(person);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldSkipTap_WhenEntityIsGiven_AndInnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.TryRemove(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldDoNothing_WhenEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Returns(expected);
			_mockTap
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryRemove(person);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndEntityIsGiven(bool expected)
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Returns(expected);
			_mockTap.Setup(u => u.Remove(person));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryRemove(person);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldSkipTap_WhenLoggerIsGiven_AndEntityIsGiven_InnerThrowsException()
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.TryRemove(person);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldLogException_WhenLoggerIsGiven_AndEntityIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			var person = new FakeEntity<int> { Id = 7, Name = "Tiffany" };

			_mockInner
				.Setup(u => u.TryRemove(person))
				.Returns(expected);
			_mockTap
				.Setup(u => u.Remove(person))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryRemove(person);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndTap_WhenIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Returns(expected);
			_mockTap.Setup(u => u.Remove(id));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryRemove(id);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldSkipTap_WhenIdIsGiven_AndInnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			Action action = () => subject.TryRemove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldDoNothing_WhenIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Returns(expected);
			_mockTap
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object);

			// Act
			var result = subject.TryRemove(id);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldCallInner_AndTap_WhenLoggerIsGiven_AndIdIsGiven(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Returns(expected);
			_mockTap.Setup(u => u.Remove(id));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryRemove(id);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Fact]
		public void TryRemove_ShouldSkipTap_WhenLoggerIsGiven_AndIdIsGiven_InnerThrowsException()
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Throws(new InvalidOperationException());

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			Action action = () => subject.TryRemove(id);

			// Assert
			action.Should().Throw<InvalidOperationException>();

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TryRemove_ShouldLogException_WhenLoggerIsGiven_AndIdIsGiven_AndTapThrowsException(bool expected)
		{
			// Arrange
			const int id = 7;

			_mockInner
				.Setup(u => u.TryRemove(id))
				.Returns(expected);
			_mockTap
				.Setup(u => u.Remove(id))
				.Throws(new InvalidOperationException());
			_mockLogger.Setup(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<InvalidOperationException>(), It.IsAny<Func<object, Exception, string>>()));

			var subject = new CommandServiceTap<FakeEntity<int>, int>(_mockInner.Object, _mockTap.Object, _mockLogger.Object);

			// Act
			var result = subject.TryRemove(id);

			// Assert
			result.Should().Be(expected);

			_mockInner.VerifyAll();
			_mockTap.VerifyAll();
			_mockLogger.VerifyAll();
		}
	}
}
