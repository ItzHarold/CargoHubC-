using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Backend.Features.Items;
using Microsoft.EntityFrameworkCore;
using Backend.Features.TransferItems;
using Backend.Response;
using FluentValidation;

namespace Backend.Features.Transfers
{
    public interface ITransferService
    {
        IEnumerable<Transfer> GetAllTransfers();
        Transfer? GetTransferById(int id);
        void AddTransfer(TransferRequest transferRequest);
        Task UpdateTransfer(int id, TransferRequest request);
        void DeleteTransfer(int id);
    }

    public class TransferService : ITransferService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IItemService _itemService;
        private readonly IValidator<Transfer> _validator;

        public TransferService(CargoHubDbContext dbContext, IItemService itemService, IValidator<Transfer> validator)
        {
            _dbContext = dbContext;
            _itemService = itemService;
            _validator = validator;
        }

        public IEnumerable<Transfer> GetAllTransfers()
        {
            if (_dbContext.Transfers != null)
            {
                return _dbContext.Transfers
                    .Include(t => t.TransferItems)  // Include related TransferItems
                    .Include(t => t.TransferFrom)   // Include related TransferFrom (Location)
                    .Include(t => t.TransferTo)     // Include related TransferTo (Location)
                    .ToList();
            }
            return new List<Transfer>();
        }


        public void AddTransfer(TransferRequest transferRequest)
        {
            // Validate that items exist and are valid
            foreach (var transferItem in transferRequest.Items)
            {
                var item = _itemService.GetItemById(transferItem.ItemUid);
                if (item == null)
                {
                    throw new KeyNotFoundException($"Item with UID {transferItem.ItemUid} not found");
                }
            }

            // Fetch TransferFrom and TransferTo locations by ID
            var transferFromLocation = _dbContext.Locations?.FirstOrDefault(l => l.Id == transferRequest.TransferFrom);
            var transferToLocation = _dbContext.Locations?.FirstOrDefault(l => l.Id == transferRequest.TransferTo);

            if (transferFromLocation == null)
            {
                throw new KeyNotFoundException($"Location with ID {transferRequest.TransferFrom} not found");
            }

            if (transferToLocation == null)
            {
                throw new KeyNotFoundException($"Location with ID {transferRequest.TransferTo} not found");
            }

            // Create a new Transfer entity
            var transfer = new Transfer
            {
                Reference = transferRequest.Reference,
                TransferFrom = transferFromLocation, // Assign the Location object
                TransferTo = transferToLocation,     // Assign the Location object
                TransferStatus = transferRequest.TransferStatus,
                CreatedAt = DateTime.Now,  // Set creation timestamp
                UpdatedAt = DateTime.Now   // Set update timestamp
            };

            // Add Transfer entity to the DB
            _dbContext.Transfers?.Add(transfer);
            _dbContext.SaveChanges();  // Save to generate the Transfer ID

            // Now, create TransferItems for the relation
            foreach (var transferItemRequest in transferRequest.Items)
            {
                // Ensure TransferItem is linked with Transfer
                var transferItem = new TransferItem
                {
                    TransferId = transfer.Id, // Link to Transfer by ID
                    ItemUid = transferItemRequest.ItemUid,  // Set item UID
                    Amount = transferItemRequest.Amount   // Set the amount for the item
                };

                _dbContext.TransferItems?.Add(transferItem);
            }

            // Save TransferItems to the DB
            _dbContext.SaveChanges();
        }

        public Transfer? GetTransferById(int id)
        {
            return _dbContext.Transfers?
                .Include(t => t.TransferItems)
                .FirstOrDefault(t => t.Id == id);
        }

        public async Task UpdateTransfer(int id, TransferRequest request)
        {
            // Retrieve the existing transfer by its ID, including TransferItems if necessary
            var existingTransfer = await _dbContext.Transfers!
                .Include(t => t.TransferItems)  // Include TransferItems if required for update
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTransfer == null)
            {
                throw new KeyNotFoundException($"Transfer with ID {id} not found.");
            }

            // Update the optional fields based on request (if provided)
            if (!string.IsNullOrEmpty(request.Reference))
            {
                existingTransfer.Reference = request.Reference;  // Update Reference if provided
            }

            if (!string.IsNullOrEmpty(request.TransferStatus))
            {
                existingTransfer.TransferStatus = request.TransferStatus;  // Update TransferStatus if provided
            }

            // Handle TransferFromLocationId update if provided
            if (request.TransferFrom.HasValue)
            {
                if (_dbContext.Locations != null)  // Ensure Locations DbSet is not null
                {
                    var transferFromLocation = await _dbContext.Locations
                        .FirstOrDefaultAsync(l => l.Id == request.TransferFrom.Value);

                    if (transferFromLocation != null)
                    {
                        existingTransfer.TransferFrom = transferFromLocation;  // Update TransferFrom location
                    }
                    else
                    {
                        throw new KeyNotFoundException("Transfer From Location not found.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Locations DbSet is not initialized.");
                }
            }

            // Handle TransferToLocationId update if provided
            if (request.TransferTo.HasValue)
            {
                if (_dbContext.Locations != null)  // Ensure Locations DbSet is not null
                {
                    var transferToLocation = await _dbContext.Locations
                        .FirstOrDefaultAsync(l => l.Id == request.TransferTo.Value);

                    if (transferToLocation != null)
                    {
                        existingTransfer.TransferTo = transferToLocation;  // Update TransferTo location
                    }
                    else
                    {
                        throw new KeyNotFoundException("Transfer To Location not found.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Locations DbSet is not initialized.");
                }
            }

            // Set the updated time to track the modification
            existingTransfer.UpdatedAt = DateTime.Now;

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
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
