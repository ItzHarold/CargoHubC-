using System.Collections.Generic;
using System.Linq;
using Backend.Features.InventoryLocations;
using Backend.Infrastructure.Database;
using Backend.Response;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Inventories
{
    public interface IInventoryService
    {
        IEnumerable<Inventory> GetAllInventories();
        Inventory? GetInventoryById(int id);
        Task<int> AddInventory(InventoryRequest inventoryRequest);
        Task UpdateInventory(int id, InventoryRequest inventoryRequest);
        void DeleteInventory(int id);
    }

    public class InventoryService : IInventoryService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Inventory> _validator;

        public InventoryService(CargoHubDbContext dbContext, IValidator<Inventory> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Inventory> GetAllInventories()
        {
            if (_dbContext.Inventories != null)
            {
                return _dbContext.Inventories.ToList();
            }
            return new List<Inventory>();
        }

        public Inventory? GetInventoryById(int id)
        {
            return _dbContext.Inventories?.Find(id);
        }

        public async Task<int> AddInventory(InventoryRequest inventoryRequest)
        {
            var inventory = new Inventory
            {
                ItemId = inventoryRequest.ItemId,
                Description = inventoryRequest.Description,
                ItemReference = inventoryRequest.ItemReference,
                LocationId = inventoryRequest.LocationId,
                TotalOnHand = inventoryRequest.TotalOnHand,
                TotalExpected = inventoryRequest.TotalExpected,
                TotalOrdered = inventoryRequest.TotalOrdered,
                TotalAllocated = inventoryRequest.TotalAllocated,
                TotalAvailable = inventoryRequest.TotalAvailable,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.Inventories?.Add(inventory);
            await _dbContext.SaveChangesAsync();

            // Create InventoryLocation records for each location
            if (inventoryRequest.LocationId != null && inventoryRequest.LocationId.Length > 0)
            {
                foreach (var locationId in inventoryRequest.LocationId)
                {
                    var inventoryLocation = new InventoryLocation
                    {
                        InventoryId = inventory.Id,
                        LocationId = locationId,
                        Amount = 0, // Set initial amount or distribute TotalOnHand across locations
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _dbContext.InventoryLocations?.Add(inventoryLocation);
                }

                await _dbContext.SaveChangesAsync();
            }

            return inventory.Id;
        }

        public async Task UpdateInventory(int id, InventoryRequest inventoryRequest)
        {
            var inventory = _dbContext.Inventories?.FirstOrDefault(i => i.Id == id);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {id} not found.");
            }

            inventory.ItemId = inventoryRequest.ItemId;
            inventory.Description = inventoryRequest.Description;
            inventory.ItemReference = inventoryRequest.ItemReference;
            inventory.LocationId = inventoryRequest.LocationId;
            inventory.TotalOnHand = inventoryRequest.TotalOnHand;
            inventory.TotalExpected = inventoryRequest.TotalExpected;
            inventory.TotalOrdered = inventoryRequest.TotalOrdered;
            inventory.TotalAllocated = inventoryRequest.TotalAllocated;
            inventory.TotalAvailable = inventoryRequest.TotalAvailable;
            inventory.UpdatedAt = DateTime.Now;

            // Update InventoryLocation records
            // Remove old locations that are no longer in the request
            var locationsToRemove = inventory.InventoryLocations
                .Where(il => !inventoryRequest.LocationId.Contains(il.LocationId))
                .ToList();

            foreach (var location in locationsToRemove)
            {
                _dbContext.InventoryLocations?.Remove(location);
            }

            // Add new locations
            foreach (var locationId in inventoryRequest.LocationId)
            {
                if (!inventory.InventoryLocations.Any(il => il.LocationId == locationId))
                {
                    var inventoryLocation = new InventoryLocation
                    {
                        InventoryId = inventory.Id,
                        LocationId = locationId,
                        Amount = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _dbContext.InventoryLocations?.Add(inventoryLocation);
                }
            }
            _dbContext.Inventories?.Update(inventory);
            await _dbContext.SaveChangesAsync();
        }


        public void DeleteInventory(int id)
        {
            var inventory = _dbContext.Inventories?.FirstOrDefault(c => c.Id == id);
            if (inventory != null)
            {
                _dbContext.Inventories?.Remove(inventory);
                _dbContext.SaveChanges();
            }
        }
    }
}
