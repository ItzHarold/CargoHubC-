using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;
using Backend.Features.Suppliers;
using Backend.Infrastructure.Database;
using Backend.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Backend.Response;

namespace Backend.Features.Suppliers.Tests
{
    public class SupplierServiceTests
    {
        private readonly SupplierService _supplierService;
        private readonly CargoHubDbContext _mockContext;

        public SupplierServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _supplierService = new SupplierService(_mockContext, null!);
        }

        [Fact]
        public void GetAllSuppliers_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _supplierService.GetAllSuppliers(null, null, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddSupplier_ValidSupplier_IncreasesSupplierCount()
        {
            // Arrange
            var supplierRequest = new SupplierRequest
            {
                Code = "SUP001",
                Name = "Supplier 1",
                Address = "123 Wijnhaven",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Test Test",
                PhoneNumber = "123-4567890",
                Reference = "REF001"
            };

            // Act
            await _supplierService.AddSupplier(supplierRequest);
            var allSuppliers = _supplierService.GetAllSuppliers(null, null, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allSuppliers);
            Assert.Contains(allSuppliers, s => s.Code == supplierRequest.Code);
        }

        [Fact]
        public void GetSupplierById_SupplierExists_ReturnsSupplier()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                Code = "SUP001",
                Name = "Supplier 1",
                Address = "123 Wijnhaven",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Test Test",
                PhoneNumber = "123-4567890",
                Reference = "REF001"
            };
            _mockContext.Suppliers.Add(supplier);
            _mockContext.SaveChanges();

            // Act
            var retrievedSupplier = _supplierService.GetSupplierById(supplier.Id);

            // Assert
            Assert.NotNull(retrievedSupplier);
            Assert.Equal(supplier.Code, retrievedSupplier?.Code);
        }

        [Fact]
        public void GetSupplierById_SupplierDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _supplierService.GetSupplierById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateSupplier_SupplierExists_UpdatesSupplierData()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                Code = "SUP001",
                Name = "Supplier 1",
                Address = "123 Wijnhaven",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Not Updated Name",
                PhoneNumber = "123-456789",
                Reference = "REF001"
            };
            _mockContext.Suppliers.Add(supplier);
            _mockContext.SaveChanges();

            var updatedSupplierRequest = new SupplierRequest
            {
                Code = "SUP002",
                Name = "Updated Supplier",
                Address = "123 Wijnhaven",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Ms Test",
                PhoneNumber = "987-6543210",
                Reference = "REF002"
            };

            // Act
            await _supplierService.UpdateSupplier(supplier.Id, updatedSupplierRequest);
            var retrievedSupplier = _supplierService.GetSupplierById(supplier.Id);

            // Assert
            Assert.NotNull(retrievedSupplier);
            Assert.Equal(updatedSupplierRequest.Code, retrievedSupplier?.Code);
            Assert.Equal(updatedSupplierRequest.Name, retrievedSupplier?.Name);
            Assert.Equal(updatedSupplierRequest.PhoneNumber, retrievedSupplier?.PhoneNumber);
        }

        [Fact]
        public void DeleteSupplier_SupplierExists_RemovesSupplier()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 2,
                Code = "SUP002",
                Name = "Supplier 2",
                Address = "123 Wijnhaven",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Test Test",
                PhoneNumber = "123-123456789",
                Reference = "REF002"
            };
            _mockContext.Suppliers.Add(supplier);
            _mockContext.SaveChanges();

            // Act
            _supplierService.DeleteSupplier(supplier.Id);
            var result = _supplierService.GetAllSuppliers(null, null, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteSupplier_SupplierDoesNotExist_NoChangesMade()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 3,
                Code = "SUP003",
                Name = "Supplier 3",
                Address = "789 Wijnhaven",
                City = "Test City",
                ZipCode = "5432JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Test Test",
                PhoneNumber = "123-123456789",
                Reference = "REF003"
            };
            _mockContext.Suppliers.Add(supplier);
            _mockContext.SaveChanges();

            // Act
            _supplierService.DeleteSupplier(999);

            // Assert
            Assert.Single(_supplierService.GetAllSuppliers(null, null, null, null, null, null, null, null, null, null, null, null));
        }
    }
}
