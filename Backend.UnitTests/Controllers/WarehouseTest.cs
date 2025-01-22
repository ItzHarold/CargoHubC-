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
