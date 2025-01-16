using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Backend.Features.Items;
using Microsoft.EntityFrameworkCore;
using Backend.Features.TransferItems;

namespace Backend.Features.Transfers
{
    public interface ITransferService
    {
        IEnumerable<Transfer> GetAllTransfers();
        Transfer? GetTransferById(int id);
        void AddTransfer(Transfer transfer);
        void UpdateTransfer(Transfer transfer);
        void DeleteTransfer(int id);
    }

    public class TransferService : ITransferService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IItemService _itemService;

        public TransferService(CargoHubDbContext dbContext, IItemService itemService)
        {
            _dbContext = dbContext;
            _itemService = itemService;
        }

        public IEnumerable<Transfer> GetAllTransfers()
        {
            return _dbContext.Transfers?
                .Include(t => t.TransferItems)
                .ToList() ?? new List<Transfer>();
        }

        public void AddTransfer(Transfer transfer)
        {
            // Validate that all items in TransferItems exist
            foreach (var transferItem in transfer.TransferItems ?? [])
            {
                var item = _itemService.GetItemById(transferItem.ItemUid);

                if (item == null)
                {
                    throw new KeyNotFoundException($"Item with UID {transferItem.ItemUid} not found");
                }

            }

            // Set the creation timestamp for the transfer
            transfer.CreatedAt = DateTime.Now;

            // Now, add the transfer to the DB
            _dbContext.Transfers?.Add(transfer);

            // Save to generate an ID for the transfer first
            _dbContext.SaveChanges();

            // Now, create TransferItems for the relation
            foreach (var transferItem in transfer.TransferItems ?? [])
            {
                // Ensure TransferItem is linked with Transfer
                var transferItemEntity = new TransferItem
                {
                    ItemUid = transferItem.ItemUid, // Link to Item by UID
                    Amount = transferItem.Amount  // Set the amount for the item
                };

                _dbContext.TransferItems?.Add(transferItemEntity);
            }
        }


        public Transfer? GetTransferById(int id)
        {
            return _dbContext.Transfers?
                .Include(t => t.TransferItems)
                .FirstOrDefault(t => t.Id == id);
        }

        public void UpdateTransfer(Transfer transfer)
        {
            // Validate that all items exist
            foreach (var transferItem in transfer.TransferItems ?? [] )
            {
                var item = _itemService.GetItemById(transferItem.ItemUid);
                if (item == null)
                {
                    throw new KeyNotFoundException($"Item with UID {transferItem.ItemUid} not found");
                }
            }

            transfer.UpdatedAt = DateTime.Now;
            _dbContext.Transfers?.Update(transfer);
            _dbContext.SaveChanges();
        }

        public void DeleteTransfer(int id)
        {
            var transfer = _dbContext.Transfers?.Find(id);
            if (transfer != null)
            {
                _dbContext.Transfers?.Remove(transfer);
                _dbContext.SaveChanges();
            }
        }
    }
}
