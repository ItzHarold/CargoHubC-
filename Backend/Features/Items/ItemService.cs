using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;

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
    }
}
