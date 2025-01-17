using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Backend.Features.Warehouses
{
    public interface IWarehouseService
    {
        IEnumerable<Warehouse> GetAllWarehouses();
        Warehouse? GetWarehouseById(int id);
        Task AddWarehouse(Warehouse warehouse);
        Task UpdateWarehouse(Warehouse warehouse);
        void DeleteWarehouse(int id);
    }

    public class WarehouseService : IWarehouseService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Warehouse> _validator;
        public WarehouseService(CargoHubDbContext dbContext, IValidator<Warehouse> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Warehouse> GetAllWarehouses()
        {
            if (_dbContext.Warehouses != null)
            {
                return _dbContext.Warehouses.ToList();
            }
            return new List<Warehouse>();
        }

        public Warehouse? GetWarehouseById(int id)
        {
            return _dbContext.Warehouses?.Find(id);
        }
        public async Task AddWarehouse(Warehouse warehouse)
        {
            var validationResult = await _validator.ValidateAsync(warehouse);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            warehouse.CreatedAt = DateTime.Now;
            _dbContext.Warehouses?.Add(warehouse);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateWarehouse(Warehouse warehouse)
        {
            var validationResult = await _validator.ValidateAsync(warehouse);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            warehouse.UpdatedAt = DateTime.Now;
            _dbContext.Warehouses?.Update(warehouse);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteWarehouse(int id)
        {
            var warehouse = _dbContext.Warehouses?.Find(id);
            if (warehouse != null)
            {
                _dbContext.Warehouses?.Remove(warehouse);
                _dbContext.SaveChanges();
            }
        }
    }
}
