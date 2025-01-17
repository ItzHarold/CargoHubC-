using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.ItemTypes
{
    public interface IItemTypeService
    {
        IEnumerable<ItemType> GetAllItemTypes();
        ItemType? GetItemTypeById(int id);
        void AddItemType(ItemType itemType);
        void UpdateItemType(int id, ItemType itemType);
        void DeleteItemType(int id);
    }

    public class ItemTypeService : IItemTypeService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<ItemType> _validator;

        public ItemTypeService(CargoHubDbContext dbContext, IValidator<ItemType> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<ItemType> GetAllItemTypes()
        {
            if (_dbContext.ItemTypes != null)
            {
                return _dbContext.ItemTypes.ToList();
            }
            return new List<ItemType>();
        }

        public ItemType? GetItemTypeById(int id)
        {
            return _dbContext.ItemTypes?.Find(id);
        }

        public void AddItemType(ItemType itemType)
        {
            var validationResult = _validator.Validate(itemType);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemType.CreatedAt = DateTime.Now;
            _dbContext.ItemTypes?.Add(itemType);
            _dbContext.SaveChanges();
        }

        public void UpdateItemType(int id, ItemType itemType)
        {
            if (id != itemType.Id)
            {
                throw new ValidationException("Item Type ID in the path does not match the ID in the body.");
            }

            var validationResult = _validator.Validate(itemType);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemType.UpdatedAt = DateTime.Now;
            _dbContext.ItemTypes?.Update(itemType);
            _dbContext.SaveChanges();
        }

        public void DeleteItemType(int id)
        {
            var itemType = _dbContext.ItemTypes?.FirstOrDefault(c => c.Id == id);
            if (itemType != null)
            {
                _dbContext.ItemTypes?.Remove(itemType);
                _dbContext.SaveChanges();
            }
        }
    }
}
