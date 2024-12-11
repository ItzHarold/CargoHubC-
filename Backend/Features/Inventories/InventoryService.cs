﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Backend.Features.Inventories
{
    public interface IInventoryService
    {
        IEnumerable<Inventory> GetAllInventories();
        Inventory? GetInventoryById(int id);
        void AddInventory(Inventory inventory);
        void UpdateInventory(Inventory inventory);
        void DeleteInventory(int id);
    }

    public class InventoryService : IInventoryService
    {
        private readonly List<Inventory> _inventories = new();

        public IEnumerable<Inventory> GetAllInventories()
        {
            return _inventories;
        }
        public Inventory? GetInventoryById(int id)
        {
            return _inventories.FirstOrDefault(c => c.Id == id);
        }

        public void AddInventory(Inventory inventory)
        {
            inventory.Id = _inventories.Count > 0 ? _inventories.Max(c => c.Id) + 1 : 1;
            _inventories.Add(inventory);
        }

        public void UpdateInventory(Inventory inventory)
        {
            var existingInventory = GetInventoryById(inventory.Id);
            if (existingInventory != null)
            {
                var updatedInventory = new Inventory
                {
                    Id = existingInventory.Id,
                    ItemId = existingInventory.ItemId,
                    Locations = existingInventory.Locations,
                    TotalOnHand = inventory.TotalOnHand,
                    TotalExpected = inventory.TotalExpected,
                    TotalOrdered = inventory.TotalOrdered,
                    TotalAllocated = inventory.TotalAllocated,
                    TotalAvailable = inventory.TotalAvailable,
                    Description = inventory.Description
                };
                _inventories[_inventories.IndexOf(existingInventory)] = updatedInventory;
            }
        }

        public void DeleteInventory(int id)
        {
            var inventory = GetInventoryById(id);
            if (inventory != null)
            {
                _inventories.Remove(inventory);
            }
        }
    }
}
