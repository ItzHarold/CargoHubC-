using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Warehouses
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        private readonly CargoHubDbContext _dbContext;

        public WarehouseValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(warehouse => warehouse.Code)
                .NotNull().WithMessage("Code is required.")
                .NotEmpty().WithMessage("Code cannot be empty.");

            RuleFor(warehouse => warehouse.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(warehouse => warehouse.Address)
                .NotNull().WithMessage("Address is required.")
                .NotEmpty().WithMessage("Address cannot be empty.");

            RuleFor(warehouse => warehouse.Zip)
                .NotNull().WithMessage("Zip is required.")
                .NotEmpty().WithMessage("Zip cannot be empty.");

            RuleFor(warehouse => warehouse.City)
                .NotNull().WithMessage("City is required.")
                .NotEmpty().WithMessage("City cannot be empty.");

            RuleFor(warehouse => warehouse.Province)
                .NotNull().WithMessage("Province is required.")
                .NotEmpty().WithMessage("Province cannot be empty.");

            RuleFor(warehouse => warehouse.Country)
                .NotNull().WithMessage("Country is required.")
                .NotEmpty().WithMessage("Country cannot be empty.");

            RuleFor(warehouse => warehouse.WarehouseContacts)
                .NotNull().WithMessage("Contacts is required.")
                .NotEmpty().WithMessage("Contacts Phone cannot be empty.");

        //     RuleFor(client => client.ContactEmail)
        //         .NotNull().WithMessage("Contact Email is required.")
        //         .NotEmpty().WithMessage("Contact Email cannot be empty.")
        //         .EmailAddress().WithMessage("Contact Email must be a valid email address.")
        //         .MustAsync(async (email, cancellationToken) => await IsUniqueEmail(email))
        //         .WithMessage("Contact Email must be unique.");
        }

        // private async Task<bool> IsUniquePhone(string phone)
        // {
        //     return !await _dbContext.Set<Client>().AnyAsync(c => c.ContactPhone == phone);
        // }

        // private async Task<bool> IsUniqueEmail(string email)
        // {
        //     return !await _dbContext.Set<Client>().AnyAsync(c => c.ContactEmail == email);
        // }
    }
}
