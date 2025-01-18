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
        IEnumerable<Supplier> GetAllSuppliers();
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

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            if (_dbContext.Suppliers != null)
            {
                // Include Items for each supplier
                return _dbContext.Suppliers
                    .Include(s => s.Items)  // Include the related Items collection
                    .ToList();
            }
            return new List<Supplier>();
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
