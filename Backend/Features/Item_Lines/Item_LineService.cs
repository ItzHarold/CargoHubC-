using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Features.FilterAndSort;

namespace Backend.Features.ItemLines
{
    public interface IItemLineService
    {
        IEnumerable<ItemLine> GetAllItemLines(Dictionary<string, string?>? filters = null, string? sortBy = null, bool sortDescending = false);
        ItemLine? GetItemLineById(int id);
        void AddItemLine(ItemLine itemLine);
        void UpdateItemLine(int id, ItemLine itemLine);
        void DeleteItemLine(int id);
    }

    public class ItemLineService : IItemLineService
    {
        private readonly List<ItemLine> _itemLines = new();

        public IEnumerable<ItemLine> GetAllItemLines(
                Dictionary<string, string?>? filters = null, 
                string? sortBy = null,
                bool sortDescending = false)
        {
            return _itemLines.AsEnumerable().FilterAndSort(filters, sortBy, sortDescending);
        }

        public void AddItemLine(ItemLine itemLine)
        {
            itemLine.id = _itemLines.Count > 0 ? _itemLines.Max(c => c.id) + 1 : 1;
            _itemLines.Add(itemLine);
        }

        public void UpdateItemLine(int id, ItemLine itemLine)
        {
            var existingItemLine = GetItemLineById(itemLine.id);
            if (existingItemLine != null)
            {
                var updatedItemLine = new ItemLine
                {
                    id = existingItemLine.id,
                    Name = itemLine.Name,
                    Description = itemLine.Description
                };
                _itemLines[_itemLines.IndexOf(existingItemLine)] = updatedItemLine;
            }
        }

        public void DeleteItemLine(int id)
        {
            var itemGroup = GetItemLineById(id);
            if (itemGroup != null)
            {
                _itemLines.Remove(itemGroup);
            }
        }

        public ItemLine? GetItemLineById(int id)
        {
            return _itemLines.FirstOrDefault(c => c.id == id);
        }
    }
}
