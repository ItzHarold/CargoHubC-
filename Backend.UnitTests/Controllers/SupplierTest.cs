using Backend.Features.Suppliers;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Suppliers.Tests
{
    public class SupplierServiceTests
    {
        private readonly SupplierService _supplierService;

        public SupplierServiceTests()
        {
            _supplierService = new SupplierService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllSuppliers_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _supplierService.GetAllSuppliers();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddSupplier_ValidSupplier_IncreasesSupplierCount()
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
                ContactName = "Test test",
                PhoneNumber = "123-4567890",
                Reference = "REF001"
            };

            // Act
            _supplierService.AddSupplier(supplier);
            var allSuppliers = _supplierService.GetAllSuppliers();

            // Assert
            Assert.Single(allSuppliers);
            Assert.Equal(supplier.Id, allSuppliers.First().Id);
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
                ContactName = "Test test",
                PhoneNumber = "123-4567890",
                Reference = "REF001"
            };
            _supplierService.AddSupplier(supplier);

            // Act
            var retrievedSupplier = _supplierService.GetSupplierById(supplier.Id);

            // Assert
            Assert.NotNull(retrievedSupplier);
            Assert.Equal(supplier.Id, retrievedSupplier?.Id);
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
        public void UpdateSupplier_SupplierExists_UpdatesSupplierData()
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
            _supplierService.AddSupplier(supplier);

            var updatedSupplier = new Supplier
            {
                Id = supplier.Id,
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
            _supplierService.UpdateSupplier(updatedSupplier);
            var retrievedSupplier = _supplierService.GetSupplierById(supplier.Id);

            // Assert
            Assert.NotNull(retrievedSupplier);
            Assert.Equal(updatedSupplier.Code, retrievedSupplier?.Code);
            Assert.Equal(updatedSupplier.Name, retrievedSupplier?.Name);
            Assert.Equal(updatedSupplier.PhoneNumber, retrievedSupplier?.PhoneNumber);
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
                ContactName = "Test test",
                PhoneNumber = "123-123456789",
                Reference = "REF002"
            };
            _supplierService.AddSupplier(supplier);

            // Act
            _supplierService.DeleteSupplier(supplier.Id);
            var result = _supplierService.GetAllSuppliers();

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
                ContactName = "Test test",
                PhoneNumber = "123-123456789",
                Reference = "3"
            };
            _supplierService.AddSupplier(supplier);

            // Act
            _supplierService.DeleteSupplier(999);

            // Assert
            Assert.Single(_supplierService.GetAllSuppliers());
        }
    }
}
