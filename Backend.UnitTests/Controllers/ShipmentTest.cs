using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;
using Backend.Features.Shipments;
using Backend.Infrastructure.Database;
using Backend.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Backend.Features.Contacts;
using FluentValidation;
using Moq;


namespace Backend.Features.Shipments.Tests
{
    public class ShipmentServiceTests
    {
        private readonly ShipmentService _shipmentService;

        private readonly CargoHubDbContext _mockContext;

        public ShipmentServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _shipmentService = new ShipmentService(_mockContext, null!);
        }

        [Fact]
        public void GetAllShipments_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _shipmentService.GetAllShipments(null, null, null, null, null, null, null, null, null, null, null, null, null);


            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddShipment_ValidShipment_IncreasesShipmentCount()
        {
            // Arrange
            var shipmentRequest = new ShipmentRequest
            {
                OrderId = 1, // Added this line
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                ShipmentDate = DateTime.Now.AddDays(4),
                ShipmentType = "Air",
                ShipmentStatus = "Shipped",
                CarrierCode = "1",
                ServiceCode = "1",
                PaymentType = "Prepaid",
                TransferMode = "Sea",
                TotalPackageCount = 5,
                TotalPackageWeight = 100,
                ShipmentItems = new List<ShipmentItemRequest>()
            };

            // Act
            await _shipmentService.AddShipment(shipmentRequest);
            var allShipments = _shipmentService.GetAllShipments(null, null, null, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allShipments);
            Assert.Contains(allShipments, s => s.CarrierCode == shipmentRequest.CarrierCode);
        }

        [Fact]
        public void GetShipmentById_ShipmentExists_ReturnsShipment()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 1,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                ShipmentDate = DateTime.Now.AddDays(4),
                ShipmentType = "Air",
                ShipmentStatus = "Shipped",
                CarrierCode = "1",
                ServiceCode = "1",
                PaymentType = "Prepaid",
                TransferMode = "Sea",
                TotalPackageCount = 5,
                TotalPackageWeight = 100
            };

            var sourceContact = new Contact
            {
                Id = 1,
                ContactName = "Source Contact",
                ContactEmail = "source@example.com",
                ContactPhone = "1234567890"
            };

            // Add source contact and shipment to the mock context
            _mockContext.Contacts.Add(sourceContact);
            _mockContext.Shipments.Add(shipment);
            _mockContext.SaveChanges();

            // Act
            var retrievedShipment = _shipmentService.GetShipmentById(shipment.Id);

            // Assert
            Assert.NotNull(retrievedShipment);
            Assert.Equal(shipment.CarrierCode, retrievedShipment?.CarrierCode);
        }


        [Fact]
        public void GetShipmentById_ShipmentDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _shipmentService.GetShipmentById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteShipment_ShipmentExists_RemovesShipment()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 2,
                SourceId = 2,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                ShipmentDate = DateTime.Now.AddDays(4),
                ShipmentType = "Air",
                ShipmentStatus = "Shipped",
                CarrierCode = "1",
                ServiceCode = "1",
                PaymentType = "Prepaid",
                TransferMode = "Sea",
                TotalPackageCount = 5,
                TotalPackageWeight = 100
            };
            _mockContext.Shipments.Add(shipment);
            _mockContext.SaveChanges();

            // Act
            _shipmentService.DeleteShipment(shipment.Id);
            var result = _shipmentService.GetAllShipments(null, null, null, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteShipment_ShipmentDoesNotExist_NoChangesMade()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 3,
                SourceId = 3,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                ShipmentDate = DateTime.Now.AddDays(4),
                ShipmentType = "Air",
                ShipmentStatus = "Shipped",
                CarrierCode = "1",
                ServiceCode = "1",
                PaymentType = "Prepaid",
                TransferMode = "Sea",
                TotalPackageCount = 5,
                TotalPackageWeight = 100
            };
            _mockContext.Shipments.Add(shipment);
            _mockContext.SaveChanges();

            // Act
            _shipmentService.DeleteShipment(999);

            // Assert
            Assert.Single(_shipmentService.GetAllShipments(null, null, null, null, null, null, null, null, null, null, null, null, null));

        }
    }
}
