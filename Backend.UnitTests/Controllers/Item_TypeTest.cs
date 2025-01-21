    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Features.ItemTypes;
    using Backend.Infrastructure.Database;
    using Backend.Requests;
    using Backend.UnitTests.Factories;
    using FluentValidation;
    using FluentValidation.Results;
    using Moq;
    using Xunit;

    namespace Backend.Features.ItemTypes.Tests
    {
        public class ItemTypeServiceTests
        {
            private readonly ItemTypeService _itemTypeService;
            private readonly Mock<IValidator<ItemType>> _mockValidator;
            private readonly CargoHubDbContext _mockContext;

            public ItemTypeServiceTests()
            {
                _mockValidator = new Mock<IValidator<ItemType>>();
                _mockContext = InMemoryDatabaseFactory.CreateMockContext();

                // Set up default behavior for the validator mock
                _mockValidator
                    .Setup(v => v.ValidateAsync(It.IsAny<ItemType>(), default))
                    .ReturnsAsync(new ValidationResult());

                _itemTypeService = new ItemTypeService(_mockContext, _mockValidator.Object);
            }

            [Fact]
            public void GetAllItemTypes_InitiallyEmpty_ReturnsEmptyList()
            {
                // Act
                var result = _itemTypeService.GetAllItemTypes(null, null, null, null);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public async Task AddItemType_ValidItemType_IncreasesItemTypeCount()
            {
                // Arrange
                var itemTypeRequest = new ItemTypeRequest
                {
                    Name = "Type A",
                    Description = "Description for Type A"
                };

                // Act
                var newItemTypeId = await _itemTypeService.AddItemType(itemTypeRequest);
                var allItemTypes = _itemTypeService.GetAllItemTypes(null, null, null, null);

                // Assert
                Assert.Single(allItemTypes);
                Assert.Contains(allItemTypes, it => it.Id == newItemTypeId && it.Name == itemTypeRequest.Name);
            }

            [Fact]
            public void GetItemTypeById_ItemTypeExists_ReturnsItemType()
            {
                // Arrange
                var itemType = new ItemType
                {
                    Name = "Type B",
                    Description = "Description for Type B"
                };

                _mockContext.ItemTypes?.Add(itemType);
                _mockContext.SaveChanges();

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
            public async Task UpdateItemType_ItemTypeExists_UpdatesItemTypeData()
            {
                // Arrange
                var itemType = new ItemType
                {
                    Id = 1,
                    Name = "Original Type",
                    Description = "Original description",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Add the initial ItemType to the mock context
                _mockContext.ItemTypes?.Add(itemType);
                _mockContext.SaveChanges();

                // Mock the validator to return valid results
                _mockValidator
                    .Setup(v => v.Validate(It.IsAny<ItemType>()))
                    .Returns(new ValidationResult()); // No validation errors

                // Update only the properties without creating a new instance
                itemType.Name = "Updated Type";
                itemType.Description = "Updated description";

                // Act
                await _itemTypeService.UpdateItemType(itemType);

                // Retrieve the updated ItemType from the service
                var retrievedItemType = _itemTypeService.GetItemTypeById(itemType.Id);

                // Assert
                Assert.NotNull(retrievedItemType);
                Assert.Equal("Updated Type", retrievedItemType?.Name);
                Assert.Equal("Updated description", retrievedItemType?.Description);
                Assert.Equal(itemType.CreatedAt, retrievedItemType?.CreatedAt); // Ensure CreatedAt is unchanged
                Assert.NotEqual(itemType.CreatedAt, retrievedItemType?.UpdatedAt); // Ensure UpdatedAt is updated
            }


            [Fact]
            public void UpdateItemType_ItemTypeDoesNotExist_ThrowsException()
            {
                // Arrange
                var updatedItemType = new ItemType
                {
                    Id = 999,
                    Name = "Nonexistent Type",
                    Description = "This type does not exist"
                };

                // Act & Assert
                Assert.ThrowsAsync<KeyNotFoundException>(async () => await _itemTypeService.UpdateItemType(updatedItemType));
            }

            [Fact]
            public void DeleteItemType_ItemTypeExists_RemovesItemType()
            {
                // Arrange
                var itemType = new ItemType
                {
                    Name = "Type C",
                    Description = "Description for Type C"
                };

                _mockContext.ItemTypes?.Add(itemType);
                _mockContext.SaveChanges();

                // Act
                _itemTypeService.DeleteItemType(itemType.Id);
                var result = _itemTypeService.GetAllItemTypes(null, null, null, null);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void DeleteItemType_ItemTypeDoesNotExist_NoChangesMade()
            {
                // Act
                _itemTypeService.DeleteItemType(999);

                // Assert
                Assert.Empty(_itemTypeService.GetAllItemTypes(null, null, null, null));
            }
        }
    }
