using Backend.Features.Warehouses;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Backend.Features.Contacts;
using Backend.UnitTests.Factories;

namespace Backend.Features.Warehouses.Tests
{
    public class WarehouseServiceTests
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseServiceTests()
        {
            _warehouseService = new WarehouseService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllWarehouses_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _warehouseService.GetAllWarehouses();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddWarehouse_ValidWarehouse_IncreasesWarehouseCount()
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
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "Contact 1",
                        ContactEmail = "contact1@example.com",
                        ContactPhone = "1234567890"
                    }
                }
            };

            // Act
            _warehouseService.AddWarehouse(warehouse);
            var allWarehouses = _warehouseService.GetAllWarehouses();

            // Assert
            Assert.Single(allWarehouses);
            Assert.Equal(warehouse.Id, allWarehouses.First().Id);
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
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "Contact 1",
                        ContactEmail = "contact1@example.com",
                        ContactPhone = "1234567890"
                    }
                }
            };
            _warehouseService.AddWarehouse(warehouse);

            // Act
            var retrievedWarehouse = _warehouseService.GetWarehouseById(warehouse.Id);

            // Assert
            Assert.NotNull(retrievedWarehouse);
            Assert.Equal(warehouse.Id, retrievedWarehouse?.Id);
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
        public void UpdateWarehouse_WarehouseExists_UpdatesWarehouseData()
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
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "Contact 1",
                        ContactEmail = "contact1@example.com",
                        ContactPhone = "1234567890"
                    }
                }
            };
            _warehouseService.AddWarehouse(warehouse);

            var updatedWarehouse = new Warehouse
            {
                Id = warehouse.Id,
                Code = "WH002",
                Name = "Warehouse Updated",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 2,
                        ContactName = "Updated Contact",
                        ContactEmail = "updatedcontact@example.com",
                        ContactPhone = "0987654321"
                    }
                }
            };

            // Act
            _warehouseService.UpdateWarehouse(updatedWarehouse);
            var retrievedWarehouse = _warehouseService.GetWarehouseById(warehouse.Id);

            // Assert
            Assert.NotNull(retrievedWarehouse);
            Assert.Equal(updatedWarehouse.Code, retrievedWarehouse?.Code);
            Assert.Equal(updatedWarehouse.Name, retrievedWarehouse?.Name);
            Assert.Equal(updatedWarehouse.Address, retrievedWarehouse?.Address);
            Assert.Equal(updatedWarehouse.Contacts.Length, retrievedWarehouse?.Contacts.Length);
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
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "Contact 2",
                        ContactEmail = "contact2@example.com",
                        ContactPhone = "0987654321"
                    }
                }
            };
            _warehouseService.AddWarehouse(warehouse);

            // Act
            _warehouseService.DeleteWarehouse(warehouse.Id);
            var result = _warehouseService.GetAllWarehouses();

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
                Country = "Nederland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "Contact 3",
                        ContactEmail = "contact3@example.com",
                        ContactPhone = "1122334455"
                    }
                }
            };
            _warehouseService.AddWarehouse(warehouse);

            // Act
            _warehouseService.DeleteWarehouse(999);

            // Assert
            Assert.Single(_warehouseService.GetAllWarehouses());
        }
    }
}
