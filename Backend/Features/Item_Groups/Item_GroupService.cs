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
        IEnumerable<ItemGroup> GetAllItemGroups(
            string? sort,
            string? direction,
            string? name,
            string? description);
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

        public IEnumerable<ItemGroup> GetAllItemGroups(
            string? sort,
            string? direction,
            string? name,
            string? description)
        {
            if (_dbContext.ItemGroups == null)
            {
                return new List<ItemGroup>();
            }
            IQueryable<ItemGroup> query = _dbContext.ItemGroups.AsQueryable();

            // Apply filtering based on query parameters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(i => i.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(i => i.Description!.Contains(description));
            }

            // Apply sorting based on the query parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "name":
                        query = direction == "desc" ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name);
                        break;
                    case "description":
                        query = direction == "desc" ? query.OrderByDescending(i => i.Description) : query.OrderBy(i => i.Description);
                        break;
                    default:
                        // Default sorting behavior (by Name)
                        query = query.OrderBy(i => i.Name);
                        break;
                }
            }

            // Return the filtered and sorted list
            return query.ToList();
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
