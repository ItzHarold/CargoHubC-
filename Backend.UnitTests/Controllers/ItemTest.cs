using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Items;
using Backend.Infrastructure.Database;
using Backend.Request;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Backend.Features.Items.Tests
{
    public class ItemServiceTests
    {
        private readonly ItemService _itemService;
        private readonly CargoHubDbContext _mockContext;

        public ItemServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _itemService = new ItemService(_mockContext);
        }

        [Fact]
        public void GetAllItems_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemService.GetAllItems(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItem_ValidItem_IncreasesItemCount()
        {
            // Arrange
            var itemRequest = new ItemRequest
            {
                Uid = "1",
                Code = "12345",
                Description = "Test item description",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };

            // Act
            _itemService.AddItem(itemRequest);
            var allItems = _itemService.GetAllItems(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allItems);
            Assert.Contains(allItems, i => i.Uid == itemRequest.Uid);
        }

        [Fact]
        public void GetItemById_ItemExists_ReturnsItem()
        {
            // Arrange
            var item = new Item
            {
                Uid = "2",
                Code = "74651",
                Description = "Another test item",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _mockContext.Items?.Add(item);
            _mockContext.SaveChanges();

            // Act
            var retrievedItem = _itemService.GetItemById(item.Uid);

            // Assert
            Assert.NotNull(retrievedItem);
            Assert.Equal(item.Uid, retrievedItem?.Uid);
        }

        [Fact]
        public void GetItemById_ItemDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _itemService.GetItemById("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateItem_ItemExists_UpdatesItemData()
        {
            // Arrange
            var item = new Item
            {
                Uid = "3",
                Code = "79845",
                Description = "Original description",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _mockContext.Items?.Add(item);
            _mockContext.SaveChanges();

            var updatedRequest = new ItemRequest
            {
                Uid = item.Uid,
                Code = "12356",
                Description = "Updated description",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN456"
            };

            // Act
            _itemService.UpdateItem(item.Uid, updatedRequest);
            var retrievedItem = _itemService.GetItemById(item.Uid);

            // Assert
            Assert.NotNull(retrievedItem);
            Assert.Equal(updatedRequest.Code, retrievedItem?.Code);
            Assert.Equal(updatedRequest.Description, retrievedItem?.Description);
        }

        [Fact]
        public void DeleteItem_ItemExists_RemovesItem()
        {
            // Arrange
            var item = new Item
            {
                Uid = "4",
                Code = "12345",
                Description = "Test item for deletion",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _mockContext.Items?.Add(item);
            _mockContext.SaveChanges();

            // Act
            _itemService.DeleteItem(item.Uid);
            var result = _itemService.GetAllItems(null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteItem_ItemDoesNotExist_NoChangesMade()
        {
            // Act
            _itemService.DeleteItem("nonexistent");

            // Assert
            Assert.Empty(_itemService.GetAllItems(null, null, null, null, null, null, null, null));
        }

        [Fact]
        public void GetItemsBySupplierId_ValidSupplierId_ReturnsItems()
        {
            // Arrange
            var item = new Item
            {
                Uid = "5",
                Code = "54321",
                Description = "Item with supplier",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                SupplierId = 2,
                SupplierPartNumber = "SPN789"
            };
            _mockContext.Items?.Add(item);
            _mockContext.SaveChanges();

            // Act
            var items = _itemService.GetItemsBySupplierId(2);

            // Assert
            Assert.Single(items);
            Assert.Contains(items, i => i.SupplierId == 2);
        }
    }
}
