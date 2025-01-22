using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.ItemGroups;
using Backend.Infrastructure.Database;
using Backend.Requests;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Backend.Features.ItemGroups.Tests
{
    public class ItemGroupServiceTests
    {
        private readonly ItemGroupService _itemGroupService;
        private readonly Mock<IValidator<ItemGroup>> _mockValidator;
        private readonly CargoHubDbContext _mockContext;

        public ItemGroupServiceTests()
        {
            _mockValidator = new Mock<IValidator<ItemGroup>>();
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();

            // Set up default behavior for the validator mock
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ItemGroup>(), default))
                .ReturnsAsync(new ValidationResult());

            _itemGroupService = new ItemGroupService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllItemGroups_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _itemGroupService.GetAllItemGroups(null, null, null, null);


            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddItemGroup_ValidItemGroup_IncreasesItemGroupCount()
        {
            // Arrange
            var itemGroupRequest = new ItemGroupRequest
            {
                Name = "Test Item Group",
                Description = "Description of test item group"
            };

            // Act
            var newItemGroupId = await _itemGroupService.AddItemGroup(itemGroupRequest);
            var allItemGroups = _itemGroupService.GetAllItemGroups(null, null, null, null);

            // Assert
            Assert.Single(allItemGroups);
            Assert.Contains(allItemGroups, ig => ig.Id == newItemGroupId && ig.Name == itemGroupRequest.Name);
        }

        [Fact]
        public void GetItemGroupById_ItemGroupExists_ReturnsItemGroup()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Name = "Test Item Group",
                Description = "Description of test item group"
            };

            _mockContext.ItemGroups?.Add(itemGroup);
            _mockContext.SaveChanges();

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
        public async Task UpdateItemGroup_ItemGroupExists_UpdatesItemGroupData()

        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 1,
                Name = "Original Item Group",

                Description = "Original description",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add the initial ItemGroup to the mock context
            _mockContext.ItemGroups?.Add(itemGroup);
            _mockContext.SaveChanges();

            // Mock the validator to return valid results
            _mockValidator
                .Setup(v => v.Validate(It.IsAny<ItemGroup>()))
                .Returns(new ValidationResult()); // No validation errors

            // Modify the existing entity directly
            itemGroup.Name = "Updated Item Group";
            itemGroup.Description = "Updated description";

            // Act
            await _itemGroupService.UpdateItemGroup(itemGroup);

            // Retrieve the updated ItemGroup from the service
            var retrievedItemGroup = _itemGroupService.GetItemGroupById(itemGroup.Id);

            // Assert
            Assert.NotNull(retrievedItemGroup);

            Assert.Equal("Updated Item Group", retrievedItemGroup?.Name);
            Assert.Equal("Updated description", retrievedItemGroup?.Description);
            Assert.Equal(itemGroup.CreatedAt, retrievedItemGroup?.CreatedAt); // Ensure CreatedAt is unchanged
            Assert.NotEqual(itemGroup.CreatedAt, retrievedItemGroup?.UpdatedAt); // Ensure UpdatedAt is updated
        }


        [Fact]
        public void UpdateItemGroup_ItemGroupDoesNotExist_ThrowsException()
        {
            // Arrange
            var updatedItemGroup = new ItemGroup
            {
                Id = 999,
                Name = "Nonexistent Item Group",
                Description = "This group does not exist"
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _itemGroupService.UpdateItemGroup(updatedItemGroup));
        }

        [Fact]
        public void DeleteItemGroup_ItemGroupExists_RemovesItemGroup()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Name = "Test Item Group",
                Description = "Description for deletion test"
            };

            _mockContext.ItemGroups?.Add(itemGroup);
            _mockContext.SaveChanges();

            // Act
            _itemGroupService.DeleteItemGroup(itemGroup.Id);
            var result = _itemGroupService.GetAllItemGroups(null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteItemGroup_ItemGroupDoesNotExist_NoChangesMade()
        {
            // Act
            _itemGroupService.DeleteItemGroup(999);

            // Assert
            Assert.Empty(_itemGroupService.GetAllItemGroups(null, null, null, null));

        }
    }
}
