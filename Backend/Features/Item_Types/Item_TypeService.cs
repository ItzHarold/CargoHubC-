using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Backend.Requests;
using FluentValidation;

namespace Backend.Features.ItemTypes
{
    public interface IItemTypeService
    {
        IEnumerable<ItemType> GetAllItemTypes();
        ItemType? GetItemTypeById(int id);
        Task<int> AddItemType(ItemTypeRequest itemTypeRequest);
        Task UpdateItemType(ItemType itemType);
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

        public async Task<int> AddItemType(ItemTypeRequest itemTypeRequest)
        {
            // var validationResult = _validator.Validate(itemGroup);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            var itemType = new ItemType()
            {
                Name = itemTypeRequest.Name,
                Description = itemTypeRequest.Description
            };

            itemType.CreatedAt = DateTime.Now;
            itemType.UpdatedAt = itemType.CreatedAt;

            _dbContext.ItemTypes?.Add(itemType);
            await _dbContext.SaveChangesAsync();
            return itemType.Id;
        }

        public async Task UpdateItemType(ItemType itemType)
        {
            var validationResult = _validator.Validate(itemType);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemType.UpdatedAt = DateTime.Now;
            _dbContext.ItemTypes?.Update(itemType);
            await _dbContext.SaveChangesAsync();
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
