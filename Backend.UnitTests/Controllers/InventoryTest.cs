using System.Collections.Generic;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Inventories.Tests
{
    public class InventoryServiceTests
    {
        private readonly InventoryService _inventoryService;

        public InventoryServiceTests()
        {
            _inventoryService = new InventoryService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllInventories_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _inventoryService.GetAllInventories();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddInventory_ValidInventory_IncreasesInventoryCount()
        {
            // Arrange
            var inventory = new Inventory
            {
                Id = 1,
                ItemId = 1,
                TotalOnHand = 10,
                TotalExpected = 20,
                TotalOrdered = 15,
                TotalAllocated = 5,
                TotalAvailable = 10,
                Description = "Test inventory",
                LocationId = [1, 2, 3]
            };

            // Act
            _inventoryService.AddInventory(inventory);
            var allInventories = _inventoryService.GetAllInventories();

            // Assert
            Assert.Single(allInventories);
            Assert.Equal(inventory.ItemId, allInventories.First().ItemId);
        }

        [Fact]
        public void GetInventoryById_InventoryExists_ReturnsInventory()
        {
            // Arrange
            var inventory = new Inventory
            {
                Id = 1,
                ItemId = 1,
                TotalOnHand = 30,
                TotalExpected = 50,
                TotalOrdered = 20,
                TotalAllocated = 10,
                TotalAvailable = 20,
                Description = "Another test inventory",
                LocationId = [1, 2, 3]
            };
            _inventoryService.AddInventory(inventory);

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
        public void UpdateInventory_InventoryExists_UpdatesInventoryData()
        {
            // Arrange
            var inventory = new Inventory
            {
                Id = 1,
                ItemId = 1,
                TotalOnHand = 50,
                TotalExpected = 70,
                TotalOrdered = 30,
                TotalAllocated = 20,
                TotalAvailable = 30,
                Description = "Original inventory",
                LocationId = [1, 2, 3]
            };
            _inventoryService.AddInventory(inventory);

            var updatedInventory = new Inventory
            {
                Id = inventory.Id,
                ItemId = 1,
                TotalOnHand = 60,
                TotalExpected = 80,
                TotalOrdered = 40,
                TotalAllocated = 30,
                TotalAvailable = 50,
                Description = "Updated inventory",
                LocationId = [1, 2, 3]
            };

            // Act
            _inventoryService.UpdateInventory(updatedInventory);
            var retrievedInventory = _inventoryService.GetInventoryById(inventory.Id);

            // Assert
            Assert.NotNull(retrievedInventory);
            Assert.Equal(updatedInventory.TotalOnHand, retrievedInventory?.TotalOnHand);
            Assert.Equal(updatedInventory.Description, retrievedInventory?.Description);
        }

        [Fact]
        public void UpdateInventory_InventoryDoesNotExist_NoChangesMade()
        {
            // Arrange
            var updatedInventory = new Inventory
            {
                Id = 999,
                ItemId = 1,
                TotalOnHand = 0,
                TotalExpected = 0,
                TotalOrdered = 0,
                TotalAllocated = 0,
                TotalAvailable = 0,
                Description = "Nonexistent inventory",
                LocationId = [1, 2, 3]
            };

            // Act
            _inventoryService.UpdateInventory(updatedInventory);

            // Assert
            Assert.Empty(_inventoryService.GetAllInventories());
        }

        [Fact]
        public void DeleteInventory_InventoryExists_RemovesInventory()
        {
            // Arrange
            var inventory = new Inventory
            {
                Id = 1,
                ItemId = 1,
                TotalOnHand = 40,
                TotalExpected = 60,
                TotalOrdered = 30,
                TotalAllocated = 10,
                TotalAvailable = 30,
                Description = "Test inventory for deletion",
                LocationId = [1, 2, 3]
            };
            _inventoryService.AddInventory(inventory);

            // Act
            _inventoryService.DeleteInventory(inventory.Id);
            var result = _inventoryService.GetAllInventories();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteInventory_InventoryDoesNotExist_NoChangesMade()
        {
            // Arrange
            var inventory = new Inventory
            {
                Id = 1,
                ItemId = 1,
                TotalOnHand = 20,
                TotalExpected = 30,
                TotalOrdered = 15,
                TotalAllocated = 5,
                TotalAvailable = 10,
                Description = "Test inventory",
                LocationId = [1, 2, 3]
            };
            _inventoryService.AddInventory(inventory);

            // Act
            _inventoryService.DeleteInventory(999);

            // Assert
            Assert.Single(_inventoryService.GetAllInventories());
        }
    }
}
