using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Suppliers
{
    public class SupplierValidator : AbstractValidator<Supplier>
    {
        private readonly CargoHubDbContext _dbContext;

        public SupplierValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(supplier => supplier.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MustAsync(BeUniqueCode).WithMessage("Code must be unique.");

            RuleFor(supplier => supplier.PhoneNumber)
                .NotEmpty().WithMessage("Contact Phone is required.")
                .MustAsync(BeUniquePhone).WithMessage("Contact Phone must be unique.");

            RuleFor(supplier => supplier.Reference)
                .NotEmpty().WithMessage("Reference is required.")
                .MustAsync(BeUniqueReference).WithMessage("Reference must be unique.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return _dbContext.Suppliers != null && !await _dbContext.Suppliers.AnyAsync(s => s.Code == code, cancellationToken);
        }

        private async Task<bool> BeUniquePhone(string phone, CancellationToken cancellationToken)
        {
            return _dbContext.Suppliers != null && !await _dbContext.Suppliers.AnyAsync(s => s.PhoneNumber == phone, cancellationToken);
        }

        private async Task<bool> BeUniqueReference(string? reference, CancellationToken cancellationToken)
        {
            return _dbContext.Suppliers != null && !await _dbContext.Suppliers.AnyAsync(s => s.Reference == reference, cancellationToken);
        }
    }
}
