using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Backend.Infrastructure.Database;
using Backend.Response;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Suppliers
{
    public interface ISupplierService
    {
        IEnumerable<Supplier> GetAllSuppliers(
            string? sort,
            string? direction,
            string? code,
            string? name,
            string? address,
            string? city,
            string? zipCode,
            string? province,
            string? country,
            string? contactName,
            string? phoneNumber,
            string? reference);
        Supplier? GetSupplierById(int id);
        Task<Supplier> AddSupplier(SupplierRequest supplierRequest);
        Task UpdateSupplier(int id, SupplierRequest supplierRequest);
        void DeleteSupplier(int id);
    }

    public class SupplierService : ISupplierService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Supplier> _validator;

        public SupplierService(CargoHubDbContext dbContext, IValidator<Supplier> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Supplier> GetAllSuppliers(
            string? sort,
            string? direction,
            string? code,
            string? name,
            string? address,
            string? city,
            string? zipCode,
            string? province,
            string? country,
            string? contactName,
            string? phoneNumber,
            string? reference)
        {
            if (_dbContext.Suppliers == null)
            {
                return new List<Supplier>();
            }
            // Start with a base query for suppliers, including related items
            var query = _dbContext.Suppliers
                .Include(s => s.Items)  // Include the related Items collection
                .AsQueryable();

            // Apply filtering based on the query parameters
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(s => s.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(s => s.Address.Contains(address));
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(s => s.City.Contains(city));
            }

            if (!string.IsNullOrEmpty(zipCode))
            {
                query = query.Where(s => s.ZipCode.Contains(zipCode));
            }

            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(s => s.Province.Contains(province));
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(s => s.Country.Contains(country));
            }

            if (!string.IsNullOrEmpty(contactName))
            {
                query = query.Where(s => s.ContactName.Contains(contactName));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query = query.Where(s => s.PhoneNumber.Contains(phoneNumber));
            }

            if (!string.IsNullOrEmpty(reference))
            {
                query = query.Where(s => s.Reference!.Contains(reference));
            }

            // Apply sorting based on the sort and direction parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "code":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Code) : query.OrderBy(s => s.Code);
                        break;
                    case "name":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                        break;
                    case "address":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Address) : query.OrderBy(s => s.Address);
                        break;
                    case "city":
                        query = direction == "desc" ? query.OrderByDescending(s => s.City) : query.OrderBy(s => s.City);
                        break;
                    case "zip_code":
                        query = direction == "desc" ? query.OrderByDescending(s => s.ZipCode) : query.OrderBy(s => s.ZipCode);
                        break;
                    case "province":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Province) : query.OrderBy(s => s.Province);
                        break;
                    case "country":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Country) : query.OrderBy(s => s.Country);
                        break;
                    case "contact_name":
                        query = direction == "desc" ? query.OrderByDescending(s => s.ContactName) : query.OrderBy(s => s.ContactName);
                        break;
                    case "phone_number":
                        query = direction == "desc" ? query.OrderByDescending(s => s.PhoneNumber) : query.OrderBy(s => s.PhoneNumber);
                        break;
                    case "reference":
                        query = direction == "desc" ? query.OrderByDescending(s => s.Reference) : query.OrderBy(s => s.Reference);
                        break;
                    default:
                        query = query.OrderBy(s => s.Name); // Default sorting by `Name`
                        break;
                }
            }

            // Execute and return the filtered and sorted query, including related items
            return query.ToList();
        }


        public Supplier? GetSupplierById(int id)
        {
            return _dbContext.Suppliers?
                .Include(s => s.Items) // Include related Items
                .FirstOrDefault(s => s.Id == id);
        }


        public async Task<Supplier> AddSupplier(SupplierRequest supplierRequest)
        {
            var supplier = new Supplier
            {
                Code = supplierRequest.Code,
                Name = supplierRequest.Name,
                Address = supplierRequest.Address,
                AddressExtra = supplierRequest.AddressExtra,
                City = supplierRequest.City,
                ZipCode = supplierRequest.ZipCode,
                Province = supplierRequest.Province,
                Country = supplierRequest.Country,
                ContactName = supplierRequest.ContactName,
                PhoneNumber = supplierRequest.PhoneNumber,
                Reference = supplierRequest.Reference
            };

            supplier.CreatedAt = DateTime.Now;

            _dbContext.Suppliers?.Add(supplier);
            await _dbContext.SaveChangesAsync();

            return supplier; // Return the newly created supplier
        }



        public async Task UpdateSupplier(int id, SupplierRequest supplierRequest)
        {
            var supplier = _dbContext.Suppliers?.FirstOrDefault(s => s.Id == id);
            if (supplier == null)
            {
                throw new KeyNotFoundException($"Supplier with ID {id} not found.");
            }

            // Validation logic commented as per your instructions
            // var validationResult = await _validator.ValidateAsync(supplier);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            supplier.Code = supplierRequest.Code;
            supplier.Name = supplierRequest.Name;
            supplier.Address = supplierRequest.Address;
            supplier.AddressExtra = supplierRequest.AddressExtra;
            supplier.City = supplierRequest.City;
            supplier.ZipCode = supplierRequest.ZipCode;
            supplier.Province = supplierRequest.Province;
            supplier.Country = supplierRequest.Country;
            supplier.ContactName = supplierRequest.ContactName;
            supplier.PhoneNumber = supplierRequest.PhoneNumber;
            supplier.Reference = supplierRequest.Reference;
            supplier.UpdatedAt = DateTime.Now;

            _dbContext.Suppliers?.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteSupplier(int id)
        {
            var supplier = _dbContext.Suppliers?.FirstOrDefault(s => s.Id == id);
            if (supplier != null)
            {
                _dbContext.Suppliers?.Remove(supplier);
                _dbContext.SaveChanges();
            }
        }
    }
}
