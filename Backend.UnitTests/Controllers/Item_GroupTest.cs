using System.Collections.Generic;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.ItemGroups.Tests
{
    public class ItemGroupServiceTests
    {
        private readonly ItemGroupService _itemGroupService;

        public ItemGroupServiceTests()
        {
            _itemGroupService = new ItemGroupService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllItemGroups_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemGroupService.GetAllItemGroups();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItemGroup_ValidItemGroup_IncreasesItemGroupCount()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 2,
                Name = "Test Item Group",
                Description = "Description of test item group"
            };

            // Act
            _itemGroupService.AddItemGroup(itemGroup);
            var allItemGroups = _itemGroupService.GetAllItemGroups();

            // Assert
            Assert.Single(allItemGroups);
            Assert.Equal(itemGroup.Name, allItemGroups.First().Name);
        }

        [Fact]
        public void GetItemGroupById_ItemGroupExists_ReturnsItemGroup()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 2,
                Name = "Test Item Group",
                Description = "Description of test item group"
            };
            _itemGroupService.AddItemGroup(itemGroup);

            // Act
            var retrievedItemGroup = _itemGroupService.GetItemGroupById(itemGroup.Id);

            // Assert
            Assert.NotNull(retrievedItemGroup);
            Assert.Equal(itemGroup.Name, retrievedItemGroup?.Name);
        }

        [Fact]
        public void GetItemGroupById_ItemGroupDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _itemGroupService.GetItemGroupById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateItemGroup_ItemGroupExists_UpdatesItemGroupData()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 1,
                Name = "Original Item Group",
                Description = "Original description"
            };
            _itemGroupService.AddItemGroup(itemGroup);

            var updatedItemGroup = new ItemGroup
            {
                Id = itemGroup.Id,
                Name = "Updated Item Group",
                Description = "Updated description"
            };

            // Act
            _itemGroupService.UpdateItemGroup(updatedItemGroup);
            var retrievedItemGroup = _itemGroupService.GetItemGroupById(itemGroup.Id);

            // Assert
            Assert.NotNull(retrievedItemGroup);
            Assert.Equal(updatedItemGroup.Name, retrievedItemGroup?.Name);
            Assert.Equal(updatedItemGroup.Description, retrievedItemGroup?.Description);
        }

        [Fact]
        public void UpdateItemGroup_ItemGroupDoesNotExist_NoChangesMade()
        {
            // Arrange
            var updatedItemGroup = new ItemGroup
            {
                Id = 999,
                Name = "Nonexistent Item Group",
                Description = "This group does not exist"
            };

            // Act
            _itemGroupService.UpdateItemGroup(updatedItemGroup);

            // Assert
            Assert.Empty(_itemGroupService.GetAllItemGroups());
        }

        [Fact]
        public void DeleteItemGroup_ItemGroupExists_RemovesItemGroup()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 1,
                Name = "Test Item Group",
                Description = "Description for deletion test"
            };
            _itemGroupService.AddItemGroup(itemGroup);

            // Act
            _itemGroupService.DeleteItemGroup(itemGroup.Id);
            var result = _itemGroupService.GetAllItemGroups();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteItemGroup_ItemGroupDoesNotExist_NoChangesMade()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 1,
                Name = "Test Item Group",
                Description = "Description for deletion test"
            };
            _itemGroupService.AddItemGroup(itemGroup);

            // Act
            _itemGroupService.DeleteItemGroup(999);

            // Assert
            Assert.Single(_itemGroupService.GetAllItemGroups());
        }
    }
}
