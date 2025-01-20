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
        IEnumerable<Warehouse> GetAllWarehouses(
            string? sort,
            string? direction,
            string? code,
            string? name,
            string? address,
            string? zip,
            string? city,
            string? province,
            string? country);
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

        public IEnumerable<Warehouse> GetAllWarehouses(
            string? sort,
            string? direction,
            string? code,
            string? name,
            string? address,
            string? zip,
            string? city,
            string? province,
            string? country)
        {
            if (_dbContext.Warehouses == null)
            {
                return new List<Warehouse>();
            }
            // Start with a base query for warehouses, including related WarehouseContacts and Contact
            var query = _dbContext.Warehouses
                .Include(w => w.WarehouseContacts)    // Include related WarehouseContacts collection
                .ThenInclude(wc => wc.Contact)       // Include related Contact for each WarehouseContact
                .AsQueryable();

            // Apply filtering based on the query parameters
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(w => w.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(w => w.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(w => w.Address.Contains(address));
            }

            if (!string.IsNullOrEmpty(zip))
            {
                query = query.Where(w => w.Zip.Contains(zip));
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(w => w.City.Contains(city));
            }

            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(w => w.Province.Contains(province));
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(w => w.Country.Contains(country));
            }

            // Apply sorting based on the sort and direction parameters
            if (!string.IsNullOrEmpty(sort))
            {
            switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "code":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Code) : query.OrderBy(w => w.Code);
                        break;
                    case "name":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name);
                        break;
                    case "address":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Address) : query.OrderBy(w => w.Address);
                        break;
                    case "zip":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Zip) : query.OrderBy(w => w.Zip);
                        break;
                    case "city":
                        query = direction == "desc" ? query.OrderByDescending(w => w.City) : query.OrderBy(w => w.City);
                        break;
                    case "province":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Province) : query.OrderBy(w => w.Province);
                        break;
                    case "country":
                        query = direction == "desc" ? query.OrderByDescending(w => w.Country) : query.OrderBy(w => w.Country);
                        break;
                    default:
                        query = query.OrderBy(w => w.Code); // Default sorting by `Code`
                        break;
                }
            }

            // Execute and return the filtered and sorted query, including related WarehouseContacts and Contacts
            return query.ToList();
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
