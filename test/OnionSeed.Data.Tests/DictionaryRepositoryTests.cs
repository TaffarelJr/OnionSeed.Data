namespace ApplCore.DataAccess.Tests
{
    using System;
    using System.Linq;
    using Development;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Types;

    [TestClass]
    public class DictionaryRepositoryTests
    {
        [TestMethod]
        public void Count_ShouldReturnTheNumberOfItemsInTheRepository()
        {
            // Arrange
            var entity = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity.SetupGet(e => e.Id).Returns(123);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();
            repository.Add(entity.Object);

            // Act, Assert
            repository.Count.Should().Be(1);
            entity.VerifyAll();
        }

        [TestMethod]
        public void Contains_ShouldThrowException_WhenItemIsNull()
        {
            // Arrange
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            Action action = () => repository.Contains(null);

            // Assert
            action.ShouldThrowContractException("because 'null' is not a valid parameter");
        }

        [TestMethod]
        public void Contains_ShouldReturnFalse_WhenRepositoryDoesNotContainItem()
        {
            // Arrange
            var entity = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity.SetupGet(e => e.Id).Returns(123);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            var result = repository.Contains(entity.Object);

            // Assert
            result.Should().BeFalse();
            entity.VerifyAll();
        }

        [TestMethod]
        public void Contains_ShouldReturnTrue_WhenRepositoryContainsItem()
        {
            // Arrange
            var entity = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity.SetupGet(e => e.Id).Returns(123);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();
            repository.Add(entity.Object);

            // Act
            var result = repository.Contains(entity.Object);

            // Assert
            result.Should().BeTrue();
            entity.VerifyAll();
        }

        [TestMethod]
        public void All_ShouldReturnQueryable()
        {
            // Arrange
            var entity0 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            var entity1 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity0.SetupGet(e => e.Id).Returns(123);
            entity1.SetupGet(e => e.Id).Returns(321);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();
            repository.Add(entity0.Object);
            repository.Add(entity1.Object);

            // Act
            var result = repository.All().ToList();

            // Assert
            result.Count.Should().Be(2);
            result[0].Should().Be(entity0.Object);
            result[1].Should().Be(entity1.Object);
            entity0.VerifyAll();
            entity1.VerifyAll();
        }

        [TestMethod]
        public void Add_ShouldThrowException_WhenItemIsNull()
        {
            // Arrange
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            Action action = () => repository.Add(null);

            // Assert
            action.ShouldThrowContractException("because 'null' is not a valid parameter");
        }

        [TestMethod]
        public void Add_ShouldAddItemsToTheRepository()
        {
            // Arrange
            var entity0 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            var entity1 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity0.SetupGet(e => e.Id).Returns(123);
            entity1.SetupGet(e => e.Id).Returns(321);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            var result0 = repository.Add(entity0.Object);
            var result1 = repository.Add(entity1.Object);

            // Assert
            result0.Should().Be(entity0.Object);
            result1.Should().Be(entity1.Object);
            repository.Count.Should().Be(2);
            var list = repository.All().ToList();
            list.Count.Should().Be(2);
            list[0].Should().Be(entity0.Object);
            list[1].Should().Be(entity1.Object);
            entity0.VerifyAll();
            entity1.VerifyAll();
        }

        [TestMethod]
        public void Remove_ShouldThrowException_WhenItemIsNull()
        {
            // Arrange
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            Action action = () => repository.Remove(null);

            // Assert
            action.ShouldThrowContractException("because 'null' is not a valid parameter");
        }

        [TestMethod]
        public void Remove_ShouldRemoveItemsFromTheRepository()
        {
            // Arrange
            var entity0 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            var entity1 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            var entity2 = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity0.SetupGet(e => e.Id).Returns(123);
            entity1.SetupGet(e => e.Id).Returns(456);
            entity2.SetupGet(e => e.Id).Returns(789);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();
            repository.Add(entity0.Object);
            repository.Add(entity1.Object);
            repository.Add(entity2.Object);

            // Act
            var result = repository.Remove(entity1.Object);

            // Assert
            result.Should().Be(entity1.Object);
            repository.Count.Should().Be(2);
            var list = repository.All().ToList();
            list.Count.Should().Be(2);
            list[0].Should().Be(entity0.Object);
            list[1].Should().Be(entity2.Object);
            entity0.VerifyAll();
            entity1.VerifyAll();
            entity2.VerifyAll();
        }

        [TestMethod]
        public void FindById_ShouldThrowException_WhenIdIsNull()
        {
            // Arrange
            var repository = new DictionaryRepository<IEntity<String>, String>();

            // Act
            Action action = () => repository.FindById(null);

            // Assert
            action.ShouldThrowContractException("because 'null' is not a valid parameter");
        }

        [TestMethod]
        public void FindById_ShouldReturnDefault_WhenRepositoryDoesNotContainItem()
        {
            // Arrange
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();

            // Act
            var result = repository.FindById(123);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void FindById_ShouldReturnItem_WhenRepositoryContainsItem()
        {
            // Arrange
            var entity = new Mock<IEntity<Int32>>(MockBehavior.Strict);
            entity.SetupGet(e => e.Id).Returns(123);
            var repository = new DictionaryRepository<IEntity<Int32>, Int32>();
            repository.Add(entity.Object);

            // Act
            var result = repository.FindById(entity.Object.Id);

            // Assert
            result.Should().Be(entity.Object);
            entity.VerifyAll();
        }
    }
}