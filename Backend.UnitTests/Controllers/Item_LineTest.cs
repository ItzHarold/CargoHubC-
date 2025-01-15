using System.Collections.Generic;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.ItemLines.Tests
{
    public class ItemLineServiceTests
    {
        private readonly ItemLineService _itemLineService;

        public ItemLineServiceTests()
        {
            _itemLineService = new ItemLineService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllItemLines_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemLineService.GetAllItemLines();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItemLine_ValidItemLine_IncreasesItemLineCount()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                id = 1,
                Name = "Test Item Line",
                Description = "Description of test item line"
            };

            // Act
            _itemLineService.AddItemLine(itemLine);
            var allItemLines = _itemLineService.GetAllItemLines();

            // Assert
            Assert.Single(allItemLines);
            Assert.Equal(itemLine.Name, allItemLines.First().Name);
        }

        [Fact]
        public void GetItemLineById_ItemLineExists_ReturnsItemLine()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                id = 1,
                Name = "Test Item Line",
                Description = "Description of test item line"
            };
            _itemLineService.AddItemLine(itemLine);

            // Act
            var retrievedItemLine = _itemLineService.GetItemLineById(itemLine.id);

            // Assert
            Assert.NotNull(retrievedItemLine);
            Assert.Equal(itemLine.Name, retrievedItemLine?.Name);
        }

        [Fact]
        public void GetItemLineById_ItemLineDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _itemLineService.GetItemLineById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateItemLine_ItemLineExists_UpdatesItemLineData()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                id = 1,
                Name = "Original Item Line",
                Description = "Original description"
            };
            _itemLineService.AddItemLine(itemLine);

            var updatedItemLine = new ItemLine
            {
                id = itemLine.id,
                Name = "Updated Item Line",
                Description = "Updated description"
            };

            // Act
            _itemLineService.UpdateItemLine(itemLine.id, updatedItemLine);
            var retrievedItemLine = _itemLineService.GetItemLineById(itemLine.id);

            // Assert
            Assert.NotNull(retrievedItemLine);
            Assert.Equal(updatedItemLine.Name, retrievedItemLine?.Name);
            Assert.Equal(updatedItemLine.Description, retrievedItemLine?.Description);
        }

        [Fact]
        public void DeleteItemLine_ItemLineExists_RemovesItemLine()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                id = 2,
                Name = "Test Item Line",
                Description = "Description for deletion test"
            };
            _itemLineService.AddItemLine(itemLine);

            // Act
            _itemLineService.DeleteItemLine(itemLine.id);
            var result = _itemLineService.GetAllItemLines();

            // Assert
            Assert.Empty(result);
        }
    }
}
