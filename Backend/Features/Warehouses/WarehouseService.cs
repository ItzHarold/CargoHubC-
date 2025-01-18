using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Backend.Requests;
using Backend.Features.WarehouseContacts;
using Backend.Features.Contacts;

namespace Backend.Features.Warehouses
{
    public interface IWarehouseService
    {
        IEnumerable<Warehouse> GetAllWarehouses();
        Warehouse? GetWarehouseById(int id);
        Task<int> AddWarehouse(WarehouseRequest warehouseRequest);
        Task UpdateWarehouse(int id, WarehouseRequest request);
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
                // Include WarehouseContacts and optionally Contact information
                return _dbContext.Warehouses
                    .Include(w => w.WarehouseContacts)    // Include WarehouseContacts
                    .ThenInclude(wc => wc.Contact)       // Optionally include Contact if needed
                    .ToList();
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
                        ContactPhone = c.ContactPhone
                    }
                }).ToList()
            };
            warehouse.CreatedAt = DateTime.Now;
            warehouse.UpdatedAt = warehouse.CreatedAt;

            _dbContext.Warehouses?.Add(warehouse);
            await _dbContext.SaveChangesAsync();
            return warehouse.Id;
        }

        public async Task UpdateWarehouse(int id, WarehouseRequest request)
        {
            var existingWarehouse = await _dbContext.Warehouses!
                .FindAsync(id) 
                ?? throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

            // Update warehouse properties
            existingWarehouse.Code = request.Code;
            existingWarehouse.Name = request.Name;
            existingWarehouse.Address = request.Address;
            existingWarehouse.Zip = request.Zip;
            existingWarehouse.City = request.City;
            existingWarehouse.Province = request.Province;
            existingWarehouse.Country = request.Country;

            // Create a new instance of the validator and indicate it's an update
            var validationResult = await _validator.ValidateAsync(existingWarehouse, options => options.IncludeRuleSets("Update"));

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            existingWarehouse.UpdatedAt = DateTime.Now;
            _dbContext.Warehouses?.Update(existingWarehouse);
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
