using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.ItemLines
{
    public interface IItemLineService
    {
        IEnumerable<ItemLine> GetAllItemLines();
        ItemLine? GetItemLineById(int id);
        void AddItemLine(ItemLine itemLine);
        void UpdateItemLine(int id, ItemLine itemLine);
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

        public IEnumerable<ItemLine> GetAllItemLines()
        {
            if (_dbContext.ItemLines != null)
            {
                return _dbContext.ItemLines.ToList();
            }
            return new List<ItemLine>();
        }

        public ItemLine? GetItemLineById(int id)
        {
            return _dbContext.ItemLines?.Find(id);
        }

        public void AddItemLine(ItemLine itemLine)
        {
            var validationResult = _validator.Validate(itemLine);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemLine.CreatedAt = DateTime.Now;
            _dbContext.ItemLines?.Add(itemLine);
            _dbContext.SaveChanges();
        }

        public void UpdateItemLine(int id, ItemLine itemLine)
        {
            if (id != itemLine.id)
            {
                throw new ValidationException("Item Line ID in the path does not match the ID in the body.");
            }

            var validationResult = _validator.Validate(itemLine);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            itemLine.UpdatedAt = DateTime.Now;
            _dbContext.ItemLines?.Update(itemLine);
            _dbContext.SaveChanges();
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
