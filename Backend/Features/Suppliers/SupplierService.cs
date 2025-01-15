using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.Suppliers
{
    public interface ISupplierService
    {
        IEnumerable<Supplier> GetAllSuppliers();
        Supplier? GetSupplierById(int id);
        Task AddSupplier(Supplier supplier);
        Task UpdateSupplier(Supplier supplier);
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
                return _dbContext.Suppliers.ToList();
            }
            return new List<Supplier>();
        }

        public Supplier? GetSupplierById(int id)
        {
            return _dbContext.Suppliers?.FirstOrDefault(s => s.Id == id);
        }

        public async Task AddSupplier(Supplier supplier)
        {
            // Validate the supplier object using FluentValidation
            var validationResult = await _validator.ValidateAsync(supplier);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            supplier.CreatedAt = DateTime.Now;
            _dbContext.Suppliers?.Add(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            // Validate the supplier object using FluentValidation
            var validationResult = await _validator.ValidateAsync(supplier);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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
