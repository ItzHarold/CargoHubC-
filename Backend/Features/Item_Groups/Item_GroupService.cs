using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Backend.Requests;
using Backend.Response;
using FluentValidation;

namespace Backend.Features.ItemGroups
{
    public interface IItemGroupService
    {
        IEnumerable<ItemGroup> GetAllItemGroups();
        ItemGroup? GetItemGroupById(int id);
        Task<int> AddItemGroup(ItemGroupRequest itemGroupRequest);
        Task UpdateItemGroup(ItemGroup itemGroup);
        void DeleteItemGroup(int id);
    }

    public class ItemGroupService : IItemGroupService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<ItemGroup> _validator;

        public ItemGroupService(CargoHubDbContext dbContext, IValidator<ItemGroup> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<ItemGroup> GetAllItemGroups()
        {
            if (_dbContext.ItemGroups != null)
            {
                return _dbContext.ItemGroups.ToList();
            }
            return new List<ItemGroup>();
        }

        public ItemGroup? GetItemGroupById(int id)
        {
            return _dbContext.ItemGroups?.Find(id);
        }

        public async Task<int> AddItemGroup(ItemGroupRequest itemGroupRequest)
        {
            // var validationResult = _validator.Validate(itemGroup);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            var itemGroup = new ItemGroup()
            {
                Name = itemGroupRequest.Name,
                Description = itemGroupRequest.Description
            };

            itemGroup.CreatedAt = DateTime.Now;
            itemGroup.UpdatedAt = itemGroup.CreatedAt;

            _dbContext.ItemGroups?.Add(itemGroup);
            await _dbContext.SaveChangesAsync();
            return itemGroup.Id;
        }

        public async Task UpdateItemGroup(ItemGroup itemGroup)
        {
            var validationResult = _validator.Validate(itemGroup);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemGroup.UpdatedAt = DateTime.Now;
            _dbContext.ItemGroups?.Update(itemGroup);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteItemGroup(int id)
        {
            var itemGroup = _dbContext.ItemGroups?.FirstOrDefault(c => c.Id == id);
            if (itemGroup != null)
            {
                _dbContext.ItemGroups?.Remove(itemGroup);
                _dbContext.SaveChanges();
            }
        }
    }
}
