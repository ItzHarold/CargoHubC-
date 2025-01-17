using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Backend.Request;

namespace Backend.Features.Items
{
    public interface IItemService
    {
        IEnumerable<Item> GetAllItems();
        Item? GetItemById(string uid);
        void AddItem(ItemRequest itemRequest);
        void UpdateItem(string uid, ItemRequest itemRequest);
        void DeleteItem(string uid);
        IEnumerable<Item> GetItemsBySupplierId(int supplierId);
    }

    public class ItemService : IItemService
    {
        private readonly CargoHubDbContext _dbContext;
        public ItemService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Item> GetAllItems()
        {
            if (_dbContext.Items != null)
            {
                return _dbContext.Items.ToList();
            }
            return new List<Item>();
        }

        public Item? GetItemById(string uid)
        {
            return _dbContext.Items?.FirstOrDefault(i => i.Uid == uid);
        }

        public void AddItem(ItemRequest itemRequest)
        {
            var item = new Item
            {
                Uid = itemRequest.Uid,
                Code = itemRequest.Code,
                Description = itemRequest.Description,
                ShortDescription = itemRequest.ShortDescription,
                UpcCode = itemRequest.UpcCode,
                ModelNumber = itemRequest.ModelNumber,
                CommodityCode = itemRequest.CommodityCode,
                ItemLineId = itemRequest.ItemLineId,
                ItemGroupId = itemRequest.ItemGroupId,
                ItemTypeId = itemRequest.ItemTypeId,
                UnitPurchaseQuantity = itemRequest.UnitPurchaseQuantity,
                UnitOrderQuantity = itemRequest.UnitOrderQuantity,
                PackOrderQuantity = itemRequest.PackOrderQuantity,
                SupplierId = itemRequest.SupplierId,
                SupplierCode = itemRequest.SupplierCode,
                SupplierPartNumber = itemRequest.SupplierPartNumber,
                CreatedAt = DateTime.Now
            };

            _dbContext.Items?.Add(item);
            _dbContext.SaveChanges();
        }

        public void UpdateItem(string uid, ItemRequest itemRequest)
        {
            var item = _dbContext.Items?.FirstOrDefault(i => i.Uid == uid);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with UID {uid} not found.");
            }

            item.Code = itemRequest.Code;
            item.Description = itemRequest.Description;
            item.ShortDescription = itemRequest.ShortDescription;
            item.UpcCode = itemRequest.UpcCode;
            item.ModelNumber = itemRequest.ModelNumber;
            item.CommodityCode = itemRequest.CommodityCode;
            item.ItemLineId = itemRequest.ItemLineId;
            item.ItemGroupId = itemRequest.ItemGroupId;
            item.ItemTypeId = itemRequest.ItemTypeId;
            item.UnitPurchaseQuantity = itemRequest.UnitPurchaseQuantity;
            item.UnitOrderQuantity = itemRequest.UnitOrderQuantity;
            item.PackOrderQuantity = itemRequest.PackOrderQuantity;
            item.SupplierId = itemRequest.SupplierId;
            item.SupplierCode = itemRequest.SupplierCode;
            item.SupplierPartNumber = itemRequest.SupplierPartNumber;
            item.UpdatedAt = DateTime.Now;

            _dbContext.Items?.Update(item);
            _dbContext.SaveChanges();
        }

        public void DeleteItem(string uid)
        {
            var item = _dbContext.Items?.FirstOrDefault(i => i.Uid == uid);
            if (item != null)
            {
                _dbContext.Items?.Remove(item);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Item> GetItemsBySupplierId(int supplierId)
        {
            // Fetch all items associated with a specific supplier
            return _dbContext.Items?.Where(i => i.SupplierId == supplierId).ToList() ?? new List<Item>();
        }
    }
}
