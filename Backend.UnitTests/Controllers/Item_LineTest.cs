using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.ItemLines;
using Backend.Infrastructure.Database;
using Backend.Requests;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Backend.Features.ItemLines.Tests
{
    public class ItemLineServiceTests
    {
        private readonly ItemLineService _itemLineService;
        private readonly Mock<IValidator<ItemLine>> _mockValidator;
        private readonly CargoHubDbContext _mockContext;

        public ItemLineServiceTests()
        {
            _mockValidator = new Mock<IValidator<ItemLine>>();
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();

            // Set up default behavior for the validator mock
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ItemLine>(), default))
                .ReturnsAsync(new ValidationResult());

            _itemLineService = new ItemLineService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllItemLines_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemLineService.GetAllItemLines(null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddItemLine_ValidItemLine_IncreasesItemLineCount()
        {
            // Arrange
            var itemLineRequest = new ItemLineRequest
            {
                Name = "Test Item Line",
                Description = "Description of test item line"
            };

            // Act
            var newItemLineId = await _itemLineService.AddItemLine(itemLineRequest);
            var allItemLines = _itemLineService.GetAllItemLines(null, null, null, null);

            // Assert
            Assert.Single(allItemLines);
            Assert.Contains(allItemLines, il => il.id == newItemLineId && il.Name == itemLineRequest.Name);
        }

        [Fact]
        public void GetItemLineById_ItemLineExists_ReturnsItemLine()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                Name = "Test Item Line",
                Description = "Description of test item line"
            };

            _mockContext.ItemLines?.Add(itemLine);
            _mockContext.SaveChanges();

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
        public async Task UpdateItemLine_ItemLineExists_UpdatesItemLineData()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                id = 1,
                Name = "Original Item Line",
                Description = "Original description",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add the initial ItemLine to the mock context
            _mockContext.ItemLines?.Add(itemLine);
            _mockContext.SaveChanges();

            // Mock the validator to return valid results
            _mockValidator
                .Setup(v => v.Validate(It.IsAny<ItemLine>()))
                .Returns(new ValidationResult()); // No validation errors

            // Modify the existing entity directly
            itemLine.Name = "Updated Item Line";
            itemLine.Description = "Updated description";

            // Act
            await _itemLineService.UpdateItemLine(itemLine);

            // Retrieve the updated ItemLine from the service
            var retrievedItemLine = _itemLineService.GetItemLineById(itemLine.id);

            // Assert
            Assert.NotNull(retrievedItemLine);
            Assert.Equal("Updated Item Line", retrievedItemLine?.Name);
            Assert.Equal("Updated description", retrievedItemLine?.Description);
            Assert.Equal(itemLine.CreatedAt, retrievedItemLine?.CreatedAt); // Ensure CreatedAt is unchanged
            Assert.NotEqual(itemLine.CreatedAt, retrievedItemLine?.UpdatedAt); // Ensure UpdatedAt is updated
        }


        [Fact]
        public void UpdateItemLine_ItemLineDoesNotExist_ThrowsException()
        {
            // Arrange
            var updatedItemLine = new ItemLine
            {
                id = 999,
                Name = "Nonexistent Item Line",
                Description = "This line does not exist"
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _itemLineService.UpdateItemLine(updatedItemLine));
        }

        [Fact]
        public void DeleteItemLine_ItemLineExists_RemovesItemLine()
        {
            // Arrange
            var itemLine = new ItemLine
            {

                Name = "Test Item Line",
                Description = "Description for deletion test"
            };

            _mockContext.ItemLines?.Add(itemLine);
            _mockContext.SaveChanges();

            // Act
            _itemLineService.DeleteItemLine(itemLine.id);
            var result = _itemLineService.GetAllItemLines(null, null, null, null);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public void DeleteItemLine_ItemLineDoesNotExist_NoChangesMade()
        {
            // Act
            _itemLineService.DeleteItemLine(999);

            // Assert
            Assert.Empty(_itemLineService.GetAllItemLines(null, null, null, null));
        }
    }
}
