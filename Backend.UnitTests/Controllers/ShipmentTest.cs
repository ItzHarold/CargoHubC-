using Backend.Features.Shipments;
using System.Linq;
using Xunit;
using Backend.Features.Items;
using Backend.UnitTests.Factories;

namespace Backend.Features.Shipments.Tests
{
    public class ShipmentServiceTests
    {
        private readonly ShipmentService _shipmentService;

        public ShipmentServiceTests()
        {
            _shipmentService = new ShipmentService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllShipments_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _shipmentService.GetAllShipments();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddShipment_ValidShipment_IncreasesShipmentCount()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 1,
                OrderId = 1,
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
                Items = [] 
            };

            // Act
            _shipmentService.AddShipment(shipment);
            var allShipments = _shipmentService.GetAllShipments();

            // Assert
            Assert.Single(allShipments);
            Assert.Equal(shipment.Id, allShipments.First().Id);
        }

        [Fact]
        public void GetShipmentById_ShipmentExists_ReturnsShipment()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 1,
                OrderId = 1,
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
                Items = [] 
            };
            _shipmentService.AddShipment(shipment);

            // Act
            var retrievedShipment = _shipmentService.GetShipmentById(shipment.Id);

            // Assert
            Assert.NotNull(retrievedShipment);
            Assert.Equal(shipment.Id, retrievedShipment?.Id);
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
        public void UpdateShipment_ShipmentExists_UpdatesShipmentData()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 1,
                OrderId = 1,
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
                Items = [] 
            };
            _shipmentService.AddShipment(shipment);

            var updatedShipment = new Shipment
            {
                Id = shipment.Id,
                OrderId = 2,
                SourceId = 2,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(5),
                ShipmentDate = DateTime.Now.AddDays(6),
                ShipmentType = "Sea",
                ShipmentStatus = "In Transit",
                CarrierCode = "2",
                ServiceCode = "2",
                PaymentType = "Collect",
                TransferMode = "Land",
                TotalPackageCount = 10,
                TotalPackageWeight = 200,
                Items = shipment.Items
            };

            // Act
            _shipmentService.UpdateShipment(updatedShipment);
            var retrievedShipment = _shipmentService.GetShipmentById(shipment.Id);

            // Assert
            Assert.NotNull(retrievedShipment);
            Assert.Equal(updatedShipment.OrderId, retrievedShipment?.OrderId);
            Assert.Equal(updatedShipment.TotalPackageCount, retrievedShipment?.TotalPackageCount);
            Assert.Equal(updatedShipment.ShipmentStatus, retrievedShipment?.ShipmentStatus);
        }

        [Fact]
        public void DeleteShipment_ShipmentExists_RemovesShipment()
        {
            // Arrange
            var shipment = new Shipment
            {
                Id = 2,
                OrderId = 2,
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
                TotalPackageWeight = 100,
                Items = [] 
            };
            _shipmentService.AddShipment(shipment);

            // Act
            _shipmentService.DeleteShipment(shipment.Id);
            var result = _shipmentService.GetAllShipments();

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
                OrderId = 3,
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
                TotalPackageWeight = 100,
                Items = []
            };
            _shipmentService.AddShipment(shipment);

            // Act
            _shipmentService.DeleteShipment(999);

            // Assert
            Assert.Single(_shipmentService.GetAllShipments());
        }
    }
}
