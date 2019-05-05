using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

#pragma warning disable IDE0039 // Use local function
#pragma warning disable UseAsyncSuffix

namespace OnionSeed.Data
{
	public class InMemoryRepositoryTests
	{
		private readonly List<FakeEntity<int>> _entities = new List<FakeEntity<int>>
		{
			new FakeEntity<int>(1, "Bill"),
			new FakeEntity<int>(2, "Jane"),
			new FakeEntity<int>(3, "Jake"),
			new FakeEntity<int>(4, "Megan")
		};

		[Fact]
		public void Constructor_ShouldCreateEmptyRepository_WhenNoDataIsGiven()
		{
			// Act
			var result = new InMemoryRepository<FakeEntity<int>, int>();

			// Assert
			result.RawData.Count.Should().Be(0);
		}

		[Fact]
		public void Constructor_ShouldCreateEmptyRepository_WhenInitialDataIsNull()
		{
			// Act
			var result = new InMemoryRepository<FakeEntity<int>, int>(null);

			// Assert
			result.RawData.Count.Should().Be(0);
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing constructor.")]
		public void Constructor_ShouldThrowException_WhenInitialDataContainsDuplicates()
		{
			// Act
			Action action = () => new InMemoryRepository<FakeEntity<int>, int>(_entities.Concat(_entities));

			// Assert
			action.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Constructor_ShouldLoadRepository_WhenInitialDataIsGiven()
		{
			// Act
			var result = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Assert
			result.RawData.Values.Should().BeEquivalentTo(_entities);
		}

		[Fact]
		public async Task GetCountAsync_ShouldReturnNumberOfItems()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.GetCountAsync().ConfigureAwait(false);

			// Assert
			result.Should().Be(_entities.Count);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnEmptyList_WhenRepositoryIsEmpty()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			var result = (await subject.GetAllAsync().ConfigureAwait(false)).ToList();

			// Assert
			result.Count.Should().Be(0);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllEntities_WhenRepositoryHasData()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = (await subject.GetAllAsync().ConfigureAwait(false)).ToList();

			// Assert
			result.Should().BeEquivalentTo(_entities);
		}

		[Fact]
		public void GetByIdAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.GetByIdAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void GetByIdAsync_ShouldThrowException_WhenWhenEntityDoesNotExist()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			Func<Task> action = async () => await subject.GetByIdAsync(27).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<KeyNotFoundException>();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.GetByIdAsync(person.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(person);
		}

		[Fact]
		public void TryGetByIdAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.TryGetByIdAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldReturnNull_WhenWhenEntityDoesNotExist()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryGetByIdAsync(27).ConfigureAwait(false);

			// Assert
			result.Should().BeNull();
		}

		[Fact]
		public async Task TryGetByIdAsync_ShouldReturnEntity_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryGetByIdAsync(person.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeSameAs(person);
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.AddAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.AddAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task AddAsync_ShouldAddEntityToRepository_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = new FakeEntity<int>(10, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			await subject.AddAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(1);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public void AddAsync_ShouldThrowException_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			Func<Task> action = async () => await subject.AddAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void AddOrUpdateAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.AddOrUpdateAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldAddEntityToRepository_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = new FakeEntity<int>(10, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			await subject.AddOrUpdateAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(1);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public async Task AddOrUpdateAsync_ShouldUpdateEntityInRepository_WhenEntityExists()
		{
			// Arrange
			var person = new FakeEntity<int>(1, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			await subject.AddOrUpdateAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(_entities.Count);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void UpdateAsync_ShouldThrowException_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.UpdateAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<KeyNotFoundException>();
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateEntityInRepository_WhenEntityExists()
		{
			// Arrange
			var person = new FakeEntity<int>(1, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			await subject.UpdateAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(_entities.Count);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.RemoveAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			await subject.RemoveAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(0);
		}

		[Fact]
		public async Task RemoveAsync_ShouldRemoveEntityFromRepository_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			await subject.RemoveAsync(person).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(_entities.Count - 1);
			subject.RawData.Should().NotContainKey(person.Id);
		}

		[Fact]
		public void RemoveAsync_ShouldThrowException_WhenIdIsGiven_AndIdIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.RemoveAsync((string)null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task RemoveAsync_ShouldDoNothing_WhenIdIsGiven_AndEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			await subject.RemoveAsync(person.Id).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(0);
		}

		[Fact]
		public async Task RemoveAsync_ShouldRemoveEntityFromRepository_WhenIdIsGiven_AndEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			await subject.RemoveAsync(person.Id).ConfigureAwait(false);

			// Assert
			subject.RawData.Count.Should().Be(_entities.Count - 1);
			subject.RawData.Should().NotContainKey(person.Id);
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void TryAddAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.TryAddAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task TryAddAsync_ShouldAddEntityToRepository_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = new FakeEntity<int>(10, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			var result = await subject.TryAddAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeTrue();
			subject.RawData.Count.Should().Be(1);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public async Task TryAddAsync_ShouldReturnFalse_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryAddAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeFalse();
			subject.RawData.Count.Should().Be(_entities.Count);
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void TryUpdateAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.TryUpdateAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			var result = await subject.TryUpdateAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeFalse();
			subject.RawData.Count.Should().Be(0);
		}

		[Fact]
		public async Task TryUpdateAsync_ShouldUpdateEntityInRepository_WhenEntityExists()
		{
			// Arrange
			var person = new FakeEntity<int>(1, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryUpdateAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeTrue();
			subject.RawData.Count.Should().Be(_entities.Count);
			subject.RawData.Should().ContainKey(person.Id);
			subject.RawData[person.Id].Should().BeSameAs(person);
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenEntityIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenIdIsNull()
		{
			// Arrange
			var person = new FakeEntity<string>(null, "Brandon");
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync(person).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			var result = await subject.TryRemoveAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeFalse();
			subject.RawData.Count.Should().Be(0);
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldRemoveEntityFromRepository_WhenEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryRemoveAsync(person).ConfigureAwait(false);

			// Assert
			result.Should().BeTrue();
			subject.RawData.Count.Should().Be(_entities.Count - 1);
			subject.RawData.Should().NotContainKey(person.Id);
		}

		[Fact]
		public void TryRemoveAsync_ShouldThrowException_WhenIdIsGiven_AndIdIsNull()
		{
			// Arrange
			var subject = new InMemoryRepository<FakeEntity<string>, string>();

			// Act
			Func<Task> action = async () => await subject.TryRemoveAsync((string)null).ConfigureAwait(false);

			// Assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldDoNothing_WhenIdIsGiven_AndEntityDoesNotExist()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>();

			// Act
			var result = await subject.TryRemoveAsync(person.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeFalse();
			subject.RawData.Count.Should().Be(0);
		}

		[Fact]
		public async Task TryRemoveAsync_ShouldRemoveEntityFromRepository_WhenIdIsGiven_AndEntityExists()
		{
			// Arrange
			var person = _entities[1];
			var subject = new InMemoryRepository<FakeEntity<int>, int>(_entities);

			// Act
			var result = await subject.TryRemoveAsync(person.Id).ConfigureAwait(false);

			// Assert
			result.Should().BeTrue();
			subject.RawData.Count.Should().Be(_entities.Count - 1);
			subject.RawData.Should().NotContainKey(person.Id);
		}
	}
}
