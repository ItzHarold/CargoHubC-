using System.Linq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Moq;
using Backend.Features.Transfers;
using Backend.Features.Items;
using Backend.Features.Locations;
using Backend.Features.TransferItems;
using Backend.Features.Warehouses;
using Backend.UnitTests.Factories;
using Backend.Infrastructure.Database;
using Backend.Requests;
using Backend.Response;


namespace Backend.Features.Transfers.Tests
{
    public class TransferServiceTests
    {
        private readonly TransferService _transferService;
        private readonly CargoHubDbContext _mockContext;

        public TransferServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _transferService = new TransferService(_mockContext, null!, null!);
        }

        [Fact]
        public void GetAllTransfers_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _transferService.GetAllTransfers(null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddTransfer_ValidTransfer_IncreasesTransferCount()
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

            var locationFrom = new Location
            {
                Id = 1,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "LOC001",
                Row = "A",
                Rack = "1",
                Shelf = "1"
            };

            var locationTo = new Location
            {
                Id = 2,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "LOC002",
                Row = "B",
                Rack = "2",
                Shelf = "2"
            };

            var item1 = new Item
            {
                Uid = "UID001",
                Code = "ITEM001",
                Description = "Item 1 Description",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                UnitPurchaseQuantity = 100,
                UnitOrderQuantity = 50,
                PackOrderQuantity = 10,
                SupplierId = 1, // Ensure SupplierId is set
                SupplierPartNumber = "SPN001" // Ensure SupplierPartNumber is set
            };

            var item2 = new Item
            {
                Uid = "UID002",
                Code = "ITEM002",
                Description = "Item 2 Description",
                ItemLineId = 2,
                ItemGroupId = 2,
                ItemTypeId = 2,
                UnitPurchaseQuantity = 200,
                UnitOrderQuantity = 100,
                PackOrderQuantity = 20,
                SupplierId = 2, // Ensure SupplierId is set
                SupplierPartNumber = "SPN002" // Ensure SupplierPartNumber is set
            };

            _mockContext.Warehouses.Add(warehouse);
            _mockContext.Locations.AddRange(locationFrom, locationTo);
            _mockContext.Items.AddRange(item1, item2);
            _mockContext.SaveChanges();

            var mockItemService = new Mock<IItemService>();
            mockItemService.Setup(service => service.GetItemById(It.IsAny<string>()))
                .Returns(new Item
                {
                    Uid = "UID001",                  // UID is set
                    Code = "ITEM001",                // Code is set
                    Description = "Item 1 Description",  // Description is set
                    SupplierId = 1,                  // SupplierId is set (required)
                    SupplierPartNumber = "SPN001"    // SupplierPartNumber is set (required)
                });

            var transferRequest = new TransferRequest
            {
                Reference = "TRF001",
                TransferFrom = locationFrom.Id,
                TransferTo = locationTo.Id,
                TransferStatus = "Pending",
                Items = new List<TransferItemRequest>
            {
                new TransferItemRequest { ItemUid = item1.Uid, Amount = 10 },
                new TransferItemRequest { ItemUid = item2.Uid, Amount = 5 }
            }
            };

            var transferService = new TransferService(_mockContext, mockItemService.Object, null!);

            // Act
            transferService.AddTransfer(transferRequest);
            var allTransfers = transferService.GetAllTransfers(null, null, null, null, null, null);

            // Assert
            Assert.Single(allTransfers);
            Assert.Equal(transferRequest.Reference, allTransfers.First().Reference);
        }
        [Fact]
        public void GetTransferById_TransferExists_ReturnsTransfer()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TRF001",
                TransferFromLocationId = 1,
                TransferToLocationId = 2,
                TransferStatus = "Pending",
                TransferItems = new List<TransferItem>
                {
                    new TransferItem { ItemUid = "UID001", Amount = 10 },
                    new TransferItem { ItemUid = "UID002", Amount = 5 }
                }
            };
            _mockContext.Transfers.Add(transfer);
            _mockContext.SaveChanges();

            // Act
            var retrievedTransfer = _transferService.GetTransferById(transfer.Id);

            // Assert
            Assert.NotNull(retrievedTransfer);
            Assert.Equal(transfer.Reference, retrievedTransfer?.Reference);
        }

        [Fact]
        public void GetTransferById_TransferDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _transferService.GetTransferById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTransfer_TransferExists_UpdatesTransferData()
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

            var locationFrom = new Location
            {
                Id = 1,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "LOC001",
                Row = "A",
                Rack = "1",
                Shelf = "1"
            };
            var locationTo = new Location
            {
                Id = 2,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                Code = "LOC002",
                Row = "B",
                Rack = "2",
                Shelf = "2"
            };

            var item1 = new Item
            {
                Uid = "UID001",
                Code = "ITEM001",
                Description = "Item 1 Description",
                ItemLineId = 1,
                ItemGroupId = 1,
                ItemTypeId = 1,
                UnitPurchaseQuantity = 100,
                UnitOrderQuantity = 50,
                PackOrderQuantity = 10,
                SupplierId = 1,
                SupplierPartNumber = "SPN001"
            };

            var item2 = new Item
            {
                Uid = "UID002",
                Code = "ITEM002",
                Description = "Item 2 Description",
                ItemLineId = 2,
                ItemGroupId = 2,
                ItemTypeId = 2,
                UnitPurchaseQuantity = 200,
                UnitOrderQuantity = 100,
                PackOrderQuantity = 20,
                SupplierId = 2,
                SupplierPartNumber = "SPN002"
            };

            _mockContext.Warehouses.Add(warehouse);
            _mockContext.Locations.AddRange(locationFrom, locationTo);
            _mockContext.Items.AddRange(item1, item2);
            _mockContext.SaveChanges();

            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TRF001",
                TransferFromLocationId = locationFrom.Id,
                TransferToLocationId = locationTo.Id,
                TransferStatus = "Pending",
                TransferItems = new List<TransferItem>
                {
                    new TransferItem { ItemUid = item1.Uid, Amount = 10 }
                }
            };

            _mockContext.Transfers.Add(transfer);
            _mockContext.SaveChanges();

            var updatedTransferRequest = new TransferRequest
            {
                Reference = "TRF002",
                TransferFrom = locationFrom.Id,
                TransferTo = locationTo.Id,
                TransferStatus = "Completed",
                Items = new List<TransferItemRequest>
                {
                    new TransferItemRequest { ItemUid = item2.Uid, Amount = 15 }
                }
            };

            // Act

            await _transferService.UpdateTransfer(transfer.Id, updatedTransferRequest);

            var retrievedTransfer = _transferService.GetTransferById(transfer.Id);

            // Assert
            Assert.NotNull(retrievedTransfer);
            Assert.Equal(updatedTransferRequest.Reference, retrievedTransfer?.Reference);
            Assert.Equal(updatedTransferRequest.TransferStatus, retrievedTransfer?.TransferStatus);
        }


        [Fact]
        public void DeleteTransfer_TransferExists_RemovesTransfer()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 2,
                Reference = "TRF002",
                TransferFromLocationId = 1,
                TransferToLocationId = 3,
                TransferStatus = "Completed",
                TransferItems = new List<TransferItem>
                {
                    new TransferItem { ItemUid = "UID001", Amount = 10 }
                }
            };
            _mockContext.Transfers.Add(transfer);
            _mockContext.SaveChanges();

            // Act
            _transferService.DeleteTransfer(transfer.Id);
            var result = _transferService.GetAllTransfers(null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteTransfer_TransferDoesNotExist_NoChangesMade()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 3,
                Reference = "TRF003",
                TransferFromLocationId = 1,
                TransferToLocationId = 2,
                TransferStatus = "Pending",
                TransferItems = new List<TransferItem>
                {
                    new TransferItem { ItemUid = "UID001", Amount = 10 }
                }
            };
            _mockContext.Transfers.Add(transfer);
            _mockContext.SaveChanges();


            // Act
            _transferService.DeleteTransfer(999);

            // Assert
            Assert.Single(_transferService.GetAllTransfers(null, null, null, null, null, null));

        }
    }
}
