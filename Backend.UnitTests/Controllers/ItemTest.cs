using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Items.Tests
{
    public class ItemServiceTests
    {
        private readonly ItemService _itemService;

        public ItemServiceTests()
        {
            _itemService = new ItemService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllItems_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemService.GetAllItems();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItem_ValidItem_IncreasesItemCount()
        {
            // Arrange
            var item = new Item
            {
                Uid = "1",
                Code = "12345",
                Description = "Test item description",
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };

            // Act
            _itemService.AddItem(item);
            var allItems = _itemService.GetAllItems();

            // Assert
            Assert.Single(allItems);
            Assert.Equal(item.Uid, allItems.First().Uid);
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
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _itemService.AddItem(item);

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
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _itemService.AddItem(item);

            var updatedItem = new Item
            {
                Uid = item.Uid,
                Code = "12356",
                Description = "Updated description",
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN456"
            };

            // Act
            _itemService.UpdateItem(item.Uid, updatedItem);
            var retrievedItem = _itemService.GetItemById(item.Uid);

            // Assert
            Assert.NotNull(retrievedItem);
            Assert.Equal(updatedItem.Code, retrievedItem?.Code);
            Assert.Equal(updatedItem.Description, retrievedItem?.Description);
        }

        [Fact]
        public void UpdateItem_ItemDoesNotExist_NoChangesMade()
        {
            // Arrange
            var updatedItem = new Item
            {
                Uid = "nonexistent",
                Code = "99999",
                Description = "This item doesn't exist",
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN999"
            };

            // Act
            _itemService.UpdateItem("nonexistent", updatedItem);

            // Assert
            Assert.Empty(_itemService.GetAllItems());
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
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _itemService.AddItem(item);

            // Act
            _itemService.DeleteItem(item.Uid);
            var result = _itemService.GetAllItems();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteItem_ItemDoesNotExist_NoChangesMade()
        {
            // Arrange
            var item = new Item
            {
                Uid = "5",
                Code = "12345",
                Description = "Test item",
                ItemLine = 1,
                ItemGroup = 1,
                ItemType = 1,
                SupplierId = 1,
                SupplierPartNumber = "SPN123"
            };
            _itemService.AddItem(item);

            // Act
            _itemService.DeleteItem("nonexistent");

            // Assert
            Assert.Single(_itemService.GetAllItems());
        }
    }
}
