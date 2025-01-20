using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Backend.Request;

namespace Backend.Features.Items
{
    public interface IItemService
    {
        IEnumerable<Item> GetAllItems(
            string? sort,
            string? direction,
            string? code,
            string? supplierPartNumber,
            int? supplierId,
            string? commodityCode,
            string? supplierCode,
            string? modelNumber);
        Item? GetItemById(string uid);
        void AddItem(ItemRequest itemRequest);
        void UpdateItem(string uid, ItemRequest itemRequest);
        void DeleteItem(string uid);
        IEnumerable<Item> GetItemsBySupplierId(int supplierId);
        IEnumerable<Item> GetItemsByItemGroupId(int itemGroupId);
        IEnumerable<Item> GetItemsByItemLineId(int itemLineId);
        IEnumerable<Item> GetItemsByItemTypeId(int itemTypeId);
    }

    public class ItemService : IItemService
    {
        private readonly CargoHubDbContext _dbContext;
        public ItemService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Item> GetAllItems(
            string? sort,
            string? direction,
            string? code,
            string? supplierPartNumber,
            int? supplierId,
            string? commodityCode,
            string? supplierCode,
            string? modelNumber)
        {
            if (_dbContext.Items == null)
            {
                return new List<Item>();
            }
            IQueryable<Item> query = _dbContext.Items.AsQueryable();

            // Apply filtering based on the query parameters
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(i => i.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(supplierPartNumber))
            {
                query = query.Where(i => i.SupplierPartNumber.Contains(supplierPartNumber));
            }

            if (supplierId.HasValue)
            {
                query = query.Where(i => i.SupplierId == supplierId);
            }

            if (!string.IsNullOrEmpty(commodityCode))
            {
                query = query.Where(i => i.CommodityCode!.Contains(commodityCode));
            }

            if (!string.IsNullOrEmpty(supplierCode))
            {
                query = query.Where(i => i.SupplierCode!.Contains(supplierCode));
            }

            if (!string.IsNullOrEmpty(modelNumber))
            {
                query = query.Where(i => i.ModelNumber!.Contains(modelNumber));
            }

            // Apply sorting based on the query parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "code":
                        query = direction == "desc" ? query.OrderByDescending(i => i.Code) : query.OrderBy(i => i.Code);
                        break;
                    case "supplierpartnumber":
                        query = direction == "desc" ? query.OrderByDescending(i => i.SupplierPartNumber) : query.OrderBy(i => i.SupplierPartNumber);
                        break;
                    case "supplierid":
                        query = direction == "desc" ? query.OrderByDescending(i => i.SupplierId) : query.OrderBy(i => i.SupplierId);
                        break;
                    case "commoditycode":
                        query = direction == "desc" ? query.OrderByDescending(i => i.CommodityCode) : query.OrderBy(i => i.CommodityCode);
                        break;
                    case "suppliercode":
                        query = direction == "desc" ? query.OrderByDescending(i => i.SupplierCode) : query.OrderBy(i => i.SupplierCode);
                        break;
                    case "modelnumber":
                        query = direction == "desc" ? query.OrderByDescending(i => i.ModelNumber) : query.OrderBy(i => i.ModelNumber);
                        break;
                    default:
                        // Default sorting behavior (by Code)
                        query = query.OrderBy(i => i.Code);
                        break;
                }
            }

            // Return the filtered and sorted list
            return query.ToList();
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

        public IEnumerable<Item> GetItemsByItemGroupId(int itemGroupId)
        {
            // Fetch all items associated with a specific item group
            return _dbContext.Items?.Where(i => i.ItemGroupId == itemGroupId).ToList() ?? new List<Item>();
        }

        public IEnumerable<Item> GetItemsByItemLineId(int itemLineId)
        {
            // Fetch all items associated with the given ItemLineId
            return _dbContext.Items?.Where(i => i.ItemLineId == itemLineId).ToList() ?? new List<Item>();
        }

        public IEnumerable<Item> GetItemsByItemTypeId(int itemTypeId)
        {
            // Fetch all items associated with the given ItemTypeId
            return _dbContext.Items?.Where(i => i.ItemTypeId == itemTypeId).ToList() ?? new List<Item>();
        }
    }
}
