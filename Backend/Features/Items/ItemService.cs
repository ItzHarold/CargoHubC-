using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using System.Globalization;

namespace Backend.Features.Items
{
    public interface IItemService
    {
        IEnumerable<Item> GetAllItems();
        Item? GetItemById(string uid);
        void AddItem(Item item);
        void UpdateItem(string uid, Item item);
        void DeleteItem(string uid);
        IEnumerable<Item> GetItemsByItemType(int itemTypeId);
        IEnumerable<Item> GetItemsByItemGroup(int itemGroupId);
        IEnumerable<Item> GetItemsByItemLine(int itemLineId);
        IEnumerable<Item> GetItemsFilteredAndSorted(
            int? itemLineId = null,
            int? itemGroupId = null,
            int? itemTypeId = null,
            int? supplierId = null,
            string? sortBy = null,
            bool sortDescending = false
        );
    }

    public class ItemService : IItemService
    {
        public List<Item> Context { get; set; } = [];

        public IEnumerable<Item> GetAllItems() => Context;

        public Item? GetItemById(string uid)
        {
            return Context.FirstOrDefault(item => item.Uid == uid);
        }

        public void AddItem(Item item)
        {
            Context.Add(item);
        }

        public void UpdateItem(string uid, Item item)
        {
            if (item.Uid != uid) return;

            int index = Context.FindIndex(i => i.Uid == uid);
            if (index == -1) return;

            Context[index] = item;
        }

        public void DeleteItem(string uid)
        {
            foreach (var item in Context.Where(item => item.Uid == uid))
            {
                Context.Remove(item);
                break;
            }
        }

        public IEnumerable<Item> GetItemsByItemType(int itemTypeId)
        {
        return Context.Where(item => item.ItemType == itemTypeId);
        }

        public IEnumerable<Item> GetItemsByItemGroup(int itemGroupId)
        {
            return Context.Where(item => item.ItemGroup == itemGroupId);
        }

        public IEnumerable<Item> GetItemsByItemLine(int itemLineId)
        {
            return Context.Where(item => item.ItemLine == itemLineId);
        }

        public IEnumerable<Item> GetItemsFilteredAndSorted(
        int? itemLineId = null,
        int? itemGroupId = null,
        int? itemTypeId = null,
        int? supplierId = null,
        string? sortBy = null,
        bool sortDescending = false
        )
        {
            var query = Context.AsQueryable();

            // Apply filters
            if (itemLineId.HasValue)
                query = query.Where(item => item.ItemLine == itemLineId.Value);
            if (itemGroupId.HasValue)
                query = query.Where(item => item.ItemGroup == itemGroupId.Value);
            if (itemTypeId.HasValue)
                query = query.Where(item => item.ItemType == itemTypeId.Value);
            if (supplierId.HasValue)
                query = query.Where(item => item.SupplierId == supplierId.Value);

            // Apply sorting
            query = sortBy != null ? sortBy.ToLower(CultureInfo.InvariantCulture) switch
            {
                "uid" => sortDescending ? query.OrderByDescending(item => item.Uid) : query.OrderBy(item => item.Uid),
                "code" => sortDescending ? query.OrderByDescending(item => item.Code) : query.OrderBy(item => item.Code),
                "upc_code" => sortDescending ? query.OrderByDescending(item => item.UpcCode) : query.OrderBy(item => item.UpcCode),
                "model_number" => sortDescending ? query.OrderByDescending(item => item.ModelNumber) : query.OrderBy(item => item.ModelNumber),
                "commodity_code" => sortDescending ? query.OrderByDescending(item => item.CommodityCode) : query.OrderBy(item => item.CommodityCode),
                "itemline" => sortDescending ? query.OrderByDescending(item => item.ItemLine) : query.OrderBy(item => item.ItemLine),
                "itemgroup" => sortDescending ? query.OrderByDescending(item => item.ItemGroup) : query.OrderBy(item => item.ItemGroup),
                "itemtype" => sortDescending ? query.OrderByDescending(item => item.ItemType) : query.OrderBy(item => item.ItemType),
                "supplier_id" => sortDescending ? query.OrderByDescending(item => item.SupplierId) : query.OrderBy(item => item.SupplierId),
                _ => query
            }

            : query;

            return query.ToList();
        }
    }
}
