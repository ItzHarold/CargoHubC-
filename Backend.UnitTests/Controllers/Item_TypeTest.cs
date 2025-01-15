using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.ItemTypes.Tests
{
    public class ItemTypeServiceTests
    {
        private readonly ItemTypeService _itemTypeService;

        public ItemTypeServiceTests()
        {
            _itemTypeService = new ItemTypeService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllItemTypes_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemTypeService.GetAllItemTypes();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItemType_ValidItemType_IncreasesItemTypeCount()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Type A",
                Description = "Description for Type A"
            };

            // Act
            _itemTypeService.AddItemType(itemType);
            var allItemTypes = _itemTypeService.GetAllItemTypes();

            // Assert
            Assert.Single(allItemTypes);
            Assert.Equal(itemType.Name, allItemTypes.First().Name);
        }

        [Fact]
        public void GetItemTypeById_ItemTypeExists_ReturnsItemType()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Type B",
                Description = "Description for Type B"
            };
            _itemTypeService.AddItemType(itemType);

            // Act
            var retrievedItemType = _itemTypeService.GetItemTypeById(itemType.Id);

            // Assert
            Assert.NotNull(retrievedItemType);
            Assert.Equal(itemType.Name, retrievedItemType?.Name);
        }

        [Fact]
        public void GetItemTypeById_ItemTypeDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _itemTypeService.GetItemTypeById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateItemType_ItemTypeExists_UpdatesItemTypeData()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Original Type",
                Description = "Original description"
            };
            _itemTypeService.AddItemType(itemType);

            var updatedItemType = new ItemType
            {
                Id = itemType.Id,
                Name = "Updated Type",
                Description = "Updated description"
            };

            // Act
            _itemTypeService.UpdateItemType(itemType.Id, updatedItemType);
            var retrievedItemType = _itemTypeService.GetItemTypeById(itemType.Id);

            // Assert
            Assert.NotNull(retrievedItemType);
            Assert.Equal(updatedItemType.Name, retrievedItemType?.Name);
            Assert.Equal(updatedItemType.Description, retrievedItemType?.Description);
        }

        [Fact]
        public void DeleteItemType_ItemTypeExists_RemovesItemType()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Type C",
                Description = "Description for Type C"
            };
            _itemTypeService.AddItemType(itemType);

            // Act
            _itemTypeService.DeleteItemType(itemType.Id);
            var result = _itemTypeService.GetAllItemTypes();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteItemType_ItemTypeDoesNotExist_NoChangesMade()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Type D",
                Description = "Description for Type D"
            };
            _itemTypeService.AddItemType(itemType);

            // Act
            _itemTypeService.DeleteItemType(999);

            // Assert
            Assert.Single(_itemTypeService.GetAllItemTypes());
        }
    }
}
