using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Requests;
using Backend.Features.WarehouseContacts;
using Backend.Features.Contacts;

namespace Backend.Features.Warehouses
{
    public interface IWarehouseService
    {
        IEnumerable<Warehouse> GetAllWarehouses();
        Warehouse? GetWarehouseById(int id);
        Task<int> AddWarehouse(WarehouseRequest warehouseRequest);
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
            return _dbContext?.Warehouses
                ?.Include(w => w.WarehouseContacts) // Simply include the navigation property (no need for `??`)
                ?.ThenInclude(c => c.Contact) // Include the related `Contact` for each `WarehouseContact`
                ?.FirstOrDefault(w => w.Id == id);
        }

        public async Task<int> AddWarehouse(WarehouseRequest warehouseRequest)
        {
            // var validationResult = await _validator.ValidateAsync(warehouse);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            var warehouse = new Warehouse()
            {
                Code = warehouseRequest.Code,
                Address = warehouseRequest.Address,
                Zip = warehouseRequest.Zip,
                Name = warehouseRequest.Name,
                City = warehouseRequest.City,
                Province = warehouseRequest.Province,
                Country = warehouseRequest.Country,
                WarehouseContacts = warehouseRequest.Contacts.Select(c => new WarehouseContact
                {
                    Contact = new Contact {
                        ContactName = c.ContactName,
                        ContactEmail = c.ContactEmail,
                        ContactPhone = c.ContactPhone,
                    }
                }).ToList()
            };
            warehouse.CreatedAt = DateTime.Now;


            _dbContext.Warehouses?.Add(warehouse);
            await _dbContext.SaveChangesAsync();
            return warehouse.Id;
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
