using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;
using Backend.Requests;

namespace Backend.Features.ItemLines
{
    public interface IItemLineService
    {
        IEnumerable<ItemLine> GetAllItemLines(
            string? sort,
            string? direction,
            string? name,
            string? description);
        ItemLine? GetItemLineById(int id);
        Task<int> AddItemLine(ItemLineRequest itemLineRequest);
        Task UpdateItemLine(ItemLine itemLine);
        void DeleteItemLine(int id);
    }

    public class ItemLineService : IItemLineService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<ItemLine> _validator;

        public ItemLineService(CargoHubDbContext dbContext, IValidator<ItemLine> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<ItemLine> GetAllItemLines(
            string? sort,
            string? direction,
            string? name,
            string? description)
        {
            if (_dbContext.ItemLines == null)
            {
                return new List<ItemLine>();
            }
            IQueryable<ItemLine> query = _dbContext.ItemLines.AsQueryable();

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

        public ItemLine? GetItemLineById(int id)
        {
            return _dbContext.ItemLines?.Find(id);
        }

        public async Task<int> AddItemLine(ItemLineRequest itemLineRequest)
        {
            // Validation (if applicable)
            // var validationResult = await _validator.ValidateAsync(itemLineRequest);
            
            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            var itemLine = new ItemLine()
            {
                Name = itemLineRequest.Name,
                Description = itemLineRequest.Description
            };

            itemLine.CreatedAt = DateTime.Now;
            itemLine.UpdatedAt = itemLine.CreatedAt;

            // Adding to the appropriate DbSet for ItemLine, not ItemGroup
            _dbContext.ItemLines?.Add(itemLine);
            await _dbContext.SaveChangesAsync();

            return itemLine.id;
        }


        public async Task UpdateItemLine(ItemLine itemLine)
        {
            var validationResult = _validator.Validate(itemLine);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemLine.UpdatedAt = DateTime.Now;
            _dbContext.ItemLines?.Update(itemLine);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteItemLine(int id)
        {
            var itemLine = _dbContext.ItemLines?.FirstOrDefault(c => c.id == id);
            if (itemLine != null)
            {
                _dbContext.ItemLines?.Remove(itemLine);
                _dbContext.SaveChanges();
            }
        }
    }
}
