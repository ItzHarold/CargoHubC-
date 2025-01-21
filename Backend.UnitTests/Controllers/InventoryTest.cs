using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Inventories;
using Backend.Infrastructure.Database;
using Backend.Response;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Backend.Features.Inventories.Tests
{
    public class InventoryServiceTests
    {
        private readonly InventoryService _inventoryService;
        private readonly Mock<IValidator<Inventory>> _mockValidator;
        private readonly CargoHubDbContext _mockContext;

        public InventoryServiceTests()
        {
            _mockValidator = new Mock<IValidator<Inventory>>();
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();

            // Set up a default behavior for the validator mock
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Inventory>(), default))
                .ReturnsAsync(new ValidationResult());

            _inventoryService = new InventoryService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllInventories_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _inventoryService.GetAllInventories(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddInventory_ValidInventory_IncreasesInventoryCount()
        {
            // Arrange
            var inventoryRequest = new InventoryRequest
            {
                ItemId = "Item1",
                TotalOnHand = 10,
                TotalExpected = 20,
                TotalOrdered = 15,
                TotalAllocated = 5,
                TotalAvailable = 10,
                Description = "Test inventory",
                LocationId = new[] { 1, 2, 3 }
            };

            // Act
            var newInventoryId = await _inventoryService.AddInventory(inventoryRequest);
            var allInventories = _inventoryService.GetAllInventories(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allInventories);
            Assert.Contains(allInventories, i => i.Id == newInventoryId && i.ItemId == inventoryRequest.ItemId);
        }

        [Fact]
        public void GetInventoryById_InventoryExists_ReturnsInventory()
        {
            // Arrange
            var inventory = new Inventory
            {
                ItemId = "Item2",
                TotalOnHand = 30,
                TotalExpected = 50,
                TotalOrdered = 20,
                TotalAllocated = 10,
                TotalAvailable = 20,
                Description = "Another test inventory",
                LocationId = new[] { 1, 2, 3 }
            };

            _mockContext.Inventories?.Add(inventory);
            _mockContext.SaveChanges();

            // Act
            var retrievedInventory = _inventoryService.GetInventoryById(inventory.Id);

            // Assert
            Assert.NotNull(retrievedInventory);
            Assert.Equal(inventory.ItemId, retrievedInventory?.ItemId);
        }

        [Fact]
        public void GetInventoryById_InventoryDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _inventoryService.GetInventoryById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateInventory_InventoryExists_UpdatesInventoryData()
        {
            // Arrange
            var inventory = new Inventory
            {
                ItemId = "Item3",
                TotalOnHand = 50,
                TotalExpected = 70,
                TotalOrdered = 30,
                TotalAllocated = 20,
                TotalAvailable = 30,
                Description = "Original inventory",
                LocationId = new[] { 1, 2, 3 }
            };

            _mockContext.Inventories?.Add(inventory);
            _mockContext.SaveChanges();

            var updatedRequest = new InventoryRequest
            {
                ItemId = "UpdatedItem3",
                TotalOnHand = 60,
                TotalExpected = 80,
                TotalOrdered = 40,
                TotalAllocated = 30,
                TotalAvailable = 50,
                Description = "Updated inventory",
                LocationId = new[] { 1, 2, 4 }
            };

            // Act
            await _inventoryService.UpdateInventory(inventory.Id, updatedRequest);
            var retrievedInventory = _inventoryService.GetInventoryById(inventory.Id);

            // Assert
            Assert.NotNull(retrievedInventory);
            Assert.Equal(updatedRequest.TotalOnHand, retrievedInventory?.TotalOnHand);
            Assert.Equal(updatedRequest.Description, retrievedInventory?.Description);
        }

        [Fact]
        public void UpdateInventory_InventoryDoesNotExist_ThrowsException()
        {
            // Arrange
            var updatedRequest = new InventoryRequest
            {
                ItemId = "NonexistentItem",
                TotalOnHand = 0,
                TotalExpected = 0,
                TotalOrdered = 0,
                TotalAllocated = 0,
                TotalAvailable = 0,
                Description = "Nonexistent inventory",
                LocationId = new[] { 1, 2, 3 }
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _inventoryService.UpdateInventory(999, updatedRequest));
        }

        [Fact]
        public void DeleteInventory_InventoryExists_RemovesInventory()
        {
            // Arrange
            var inventory = new Inventory
            {
                ItemId = "Item4",
                TotalOnHand = 40,
                TotalExpected = 60,
                TotalOrdered = 30,
                TotalAllocated = 10,
                TotalAvailable = 30,
                Description = "Test inventory for deletion",
                LocationId = new[] { 1, 2, 3 }
            };

            _mockContext.Inventories?.Add(inventory);
            _mockContext.SaveChanges();

            // Act
            _inventoryService.DeleteInventory(inventory.Id);
            var result = _inventoryService.GetAllInventories(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteInventory_InventoryDoesNotExist_NoChangesMade()
        {
            // Act
            _inventoryService.DeleteInventory(999);

            // Assert
            Assert.Empty(_inventoryService.GetAllInventories(null, null, null, null, null, null, null, null));
        }
    }
}
