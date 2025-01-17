using System.Collections.Generic;
using System.Linq;
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
        Task UpdateInventory(Inventory inventory);
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
            // Validation logic commented as per your instructions
            // var validationResult = await _validator.ValidateAsync(inventory);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

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
            };

            inventory.UpdatedAt = inventory.CreatedAt;

            _dbContext.Inventories?.Add(inventory);
            await _dbContext.SaveChangesAsync();
            return inventory.Id;
        }

        public async Task UpdateInventory(Inventory inventory)
        {
            // Validation logic commented as per your instructions
            // var validationResult = await _validator.ValidateAsync(inventory);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            inventory.UpdatedAt = DateTime.Now;
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
