using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Features.FilterAndSort;

namespace Backend.Features.ItemGroups
{
    public interface IItemGroupService
    {
        IEnumerable<ItemGroup> GetAllItemGroups(Dictionary<string, string?>? filters = null, string? sortBy = null, bool sortDescending = false);
        ItemGroup? GetItemGroupById(int id);
        void AddItemGroup(ItemGroup itemGroup);
        void UpdateItemGroup(ItemGroup itemGroup);
        void DeleteItemGroup(int id);
    }

    public class ItemGroupService : IItemGroupService
    {
        private readonly List<ItemGroup> _itemGroups = new();

        public IEnumerable<ItemGroup> GetAllItemGroups(
                Dictionary<string, string?>? filters = null, 
                string? sortBy = null,
                bool sortDescending = false)
        {
            return _itemGroups.AsEnumerable().FilterAndSort(filters, sortBy, sortDescending);
        }

        public ItemGroup? GetItemGroupById(int id)
        {
            return _itemGroups.FirstOrDefault(c => c.Id == id);
        }

        public void AddItemGroup(ItemGroup itemGroup)
        {
            itemGroup.Id = _itemGroups.Count > 0 ? _itemGroups.Max(c => c.Id) + 1 : 1;
            _itemGroups.Add(itemGroup);
        }

        public void UpdateItemGroup(ItemGroup itemGroup)
        {
            var existingItemGroup = GetItemGroupById(itemGroup.Id);
            if (existingItemGroup != null)
            {
                var updatedItemGroup = new ItemGroup
                {
                    Id = existingItemGroup.Id,
                    Name = itemGroup.Name,
                    Description = itemGroup.Description
                };
                _itemGroups[_itemGroups.IndexOf(existingItemGroup)] = updatedItemGroup;
            }
        }

        public void DeleteItemGroup(int id)
        {
            var itemGroup = GetItemGroupById(id);
            if (itemGroup != null)
            {
                _itemGroups.Remove(itemGroup);
            }
        }

    }
}
