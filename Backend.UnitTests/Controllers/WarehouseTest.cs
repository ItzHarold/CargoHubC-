using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;
using Backend.Features.Warehouses;
using Backend.Infrastructure.Database;
using Backend.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Features.Contacts;
using Backend.Features.WarehouseContacts;
using FluentValidation;
using Moq;

namespace Backend.Features.Warehouses.Tests
{
    public class WarehouseServiceTests
    {
        private readonly WarehouseService _warehouseService;
        private readonly CargoHubDbContext _mockContext;
        private readonly Mock<IValidator<Warehouse>> _mockValidator;

        public WarehouseServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _mockValidator = new Mock<IValidator<Warehouse>>();

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Warehouse>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _warehouseService = new WarehouseService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllWarehouses_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _warehouseService.GetAllWarehouses(null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddWarehouse_ValidWarehouse_IncreasesWarehouseCount()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Contact 1",
                ContactEmail = "contact1@example.com",
                ContactPhone = "1234567890"
            };

            _mockContext.Contacts.Add(contact);
            _mockContext.SaveChanges();

            var warehouseRequest = new WarehouseRequest
            {
                Code = "WH001",
                Name = "Warehouse 1",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland",
                Contacts = new List<ContactRequest>
                {
                    new ContactRequest
                    {
                        ContactName = "Contact 1",
                        ContactEmail = "contact1@example.com",
                        ContactPhone = "1234567890"
                    }
                }
            };

            // Act
            await _warehouseService.AddWarehouse(warehouseRequest);
            var allWarehouses = _warehouseService.GetAllWarehouses(null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allWarehouses);
            Assert.Contains(allWarehouses, w => w.Code == warehouseRequest.Code);
        }

        [Fact]
        public void GetWarehouseById_WarehouseExists_ReturnsWarehouse()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Warehouse 1",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland"
            };
            _mockContext.Warehouses.Add(warehouse);
            _mockContext.SaveChanges();

            // Act
            var retrievedWarehouse = _warehouseService.GetWarehouseById(warehouse.Id);

            // Assert
            Assert.NotNull(retrievedWarehouse);
            Assert.Equal(warehouse.Code, retrievedWarehouse?.Code);
        }

        [Fact]
        public void GetWarehouseById_WarehouseDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _warehouseService.GetWarehouseById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateWarehouse_WarehouseExists_UpdatesWarehouseData()
        {
            // Arrange: Create initial contact and warehouse
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Original Contact",
                ContactEmail = "original@example.com",
                ContactPhone = "1234567890"
            };
            _mockContext.Contacts.Add(contact);

            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Warehouse 1",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland",
                WarehouseContacts = new List<WarehouseContact>
                {
                    new WarehouseContact { Contact = contact }
                }
            };
            _mockContext.Warehouses.Add(warehouse);
            _mockContext.SaveChanges();

            // Create the updated contact
            var updatedContact = new Contact
            {
                Id = 2,
                ContactName = "Updated Contact",
                ContactEmail = "updated@example.com",
                ContactPhone = "9876543210"
            };
            _mockContext.Contacts.Add(updatedContact);
            _mockContext.SaveChanges();

            // Updated warehouse request
            var updatedWarehouseRequest = new WarehouseRequest
            {
                Code = "WH002",
                Name = "Updated Warehouse",
                Address = "20 Nieuwe Haven",
                Zip = "5678AB",
                City = "Amsterdam",
                Province = "Noord-Holland",
                Country = "Nederland",
                Contacts = new List<ContactRequest>
                {
                    new ContactRequest
                    {
                        ContactName = "Updated Contact",
                        ContactEmail = "updated@example.com",
                        ContactPhone = "9876543210"
                    }
                }
            };

            // Act: Update the warehouse
            await _warehouseService.UpdateWarehouse(warehouse.Id, updatedWarehouseRequest);

            // Assert: Verify the updated warehouse
            var retrievedWarehouse = _warehouseService.GetWarehouseById(warehouse.Id);

            Assert.NotNull(retrievedWarehouse);
            Assert.Equal(updatedWarehouseRequest.Code, retrievedWarehouse?.Code);
            Assert.Equal(updatedWarehouseRequest.Name, retrievedWarehouse?.Name);
            Assert.Equal(updatedWarehouseRequest.Address, retrievedWarehouse?.Address);
            Assert.Equal(updatedWarehouseRequest.Zip, retrievedWarehouse?.Zip);
            Assert.Equal(updatedWarehouseRequest.City, retrievedWarehouse?.City);
            Assert.Equal(updatedWarehouseRequest.Province, retrievedWarehouse?.Province);
            Assert.Equal(updatedWarehouseRequest.Country, retrievedWarehouse?.Country);

            // Verify contacts
            Assert.NotNull(retrievedWarehouse?.WarehouseContacts);
            Assert.Single(retrievedWarehouse?.WarehouseContacts);
            Assert.Equal("Updated Contact", retrievedWarehouse?.WarehouseContacts.First().Contact.ContactName);
            Assert.Equal("updated@example.com", retrievedWarehouse?.WarehouseContacts.First().Contact.ContactEmail);
        }





        [Fact]
        public void DeleteWarehouse_WarehouseExists_RemovesWarehouse()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                Id = 2,
                Code = "WH002",
                Name = "Warehouse 2",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland"
            };
            _mockContext.Warehouses.Add(warehouse);
            _mockContext.SaveChanges();

            // Act
            _warehouseService.DeleteWarehouse(warehouse.Id);
            var result = _warehouseService.GetAllWarehouses(null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteWarehouse_WarehouseDoesNotExist_NoChangesMade()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                Id = 3,
                Code = "WH003",
                Name = "Warehouse 3",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland"
            };
            _mockContext.Warehouses.Add(warehouse);
            _mockContext.SaveChanges();

            // Act
            _warehouseService.DeleteWarehouse(999);

            // Assert
            Assert.Single(_warehouseService.GetAllWarehouses(null, null, null, null, null, null, null, null, null));
        }
    }
}
