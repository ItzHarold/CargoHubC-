using Backend.Features.Transfers;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Backend.Features.Items;
using Backend.UnitTests.Factories;

namespace Backend.Features.Transfers.Tests
{
    public class TransferServiceTests
    {
        private readonly TransferService _transferService;

        public TransferServiceTests()
        {
            _transferService = new TransferService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllTransfers_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _transferService.GetAllTransfers();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddTransfer_ValidTransfer_IncreasesTransferCount()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TRF001",
                TransferFrom = 1,
                TransferTo = 2,
                TransferStatus = "Pending",
                Items =
                [
                    new Item
                    {
                        Uid = "UID001",
                        Code = "ITEM001",
                        Description = "Item 1 Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN001"
                    },
                    new Item
                    {
                        Uid = "UID002",
                        Code = "ITEM002",
                        Description = "Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN002"
                    }
                ]
            };

            // Act
            _transferService.AddTransfer(transfer);
            var allTransfers = _transferService.GetAllTransfers();

            // Assert
            Assert.Single(allTransfers);
            Assert.Equal(transfer.Id, allTransfers.First().Id);
        }

        [Fact]
        public void GetTransferById_TransferExists_ReturnsTransfer()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TRF001",
                TransferFrom = 1,
                TransferTo = 2,
                TransferStatus = "Pending",
                Items =
                [
                    new Item
                    {
                        Uid = "UID001", 
                        Code = "ITEM001",
                        Description = "Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN001"
                    }
                ]
            };
            _transferService.AddTransfer(transfer);

            // Act
            var retrievedTransfer = _transferService.GetTransferById(transfer.Id);

            // Assert
            Assert.NotNull(retrievedTransfer);
            Assert.Equal(transfer.Id, retrievedTransfer?.Id);
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
        public void UpdateTransfer_TransferExists_UpdatesTransferData()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TRF001",
                TransferFrom = 1,
                TransferTo = 2,
                TransferStatus = "Pending",
                Items =
                [
                    new Item
                    {
                        Uid = "UID001", 
                        Code = "ITEM001",
                        Description = "Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN001"
                    }
                ]
            };
            _transferService.AddTransfer(transfer);

            var updatedTransfer = new Transfer
            {
                Id = transfer.Id,
                Reference = "TRF002",
                TransferFrom = 2,
                TransferTo = 3,
                TransferStatus = "Completed",
                Items =
                [
                    new Item
                    {
                        Uid = "UID002", 
                        Code = "ITEM002",
                        Description = "Updated Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN002"
                    }
                ]
            };

            // Act
            _transferService.UpdateTransfer(updatedTransfer);
            var retrievedTransfer = _transferService.GetTransferById(transfer.Id);

            // Assert
            Assert.NotNull(retrievedTransfer);
            Assert.Equal(updatedTransfer.Reference, retrievedTransfer?.Reference);
            Assert.Equal(updatedTransfer.TransferStatus, retrievedTransfer?.TransferStatus);
            Assert.Equal(updatedTransfer.Items.Count, retrievedTransfer?.Items.Count);
        }

        [Fact]
        public void DeleteTransfer_TransferExists_RemovesTransfer()
        {
            // Arrange
            var transfer = new Transfer
            {
                Id = 2,
                Reference = "TRF002",
                TransferFrom = 1,
                TransferTo = 3,
                TransferStatus = "Completed",
                Items =
                [
                    new Item
                    {
                        Uid = "UID001", 
                        Code = "ITEM001",
                        Description = "Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN001"
                    }
                ]
            };
            _transferService.AddTransfer(transfer);

            // Act
            _transferService.DeleteTransfer(transfer.Id);
            var result = _transferService.GetAllTransfers();

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
                TransferFrom = 1,
                TransferTo = 2,
                TransferStatus = "Pending",
                Items =
                [
                    new Item
                    {
                        Uid = "UID001", 
                        Code = "ITEM001",
                        Description = "Item Description",
                        ItemLine = 1,
                        ItemGroup = 1,
                        ItemType = 1,
                        SupplierId = 1,
                        SupplierPartNumber = "SPN001"
                    }
                ]
            };
            _transferService.AddTransfer(transfer);

            // Act
            _transferService.DeleteTransfer(999);

            // Assert
            Assert.Single(_transferService.GetAllTransfers());
        }
    }
}
