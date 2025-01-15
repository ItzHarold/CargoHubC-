using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Clients
{
    public class ClientValidator : AbstractValidator<Client>
    {
        private readonly CargoHubDbContext _dbContext;

        public ClientValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(client => client.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(client => client.Address)
                .NotNull().WithMessage("Address is required.")
                .NotEmpty().WithMessage("Address cannot be empty.");

            RuleFor(client => client.City)
                .NotNull().WithMessage("City is required.")
                .NotEmpty().WithMessage("City cannot be empty.");

            RuleFor(client => client.ZipCode)
                .NotNull().WithMessage("Zip Code is required.")
                .NotEmpty().WithMessage("Zip Code cannot be empty.");

            RuleFor(client => client.Province)
                .NotNull().WithMessage("Province is required.")
                .NotEmpty().WithMessage("Province cannot be empty.");

            RuleFor(client => client.Country)
                .NotNull().WithMessage("Country is required.")
                .NotEmpty().WithMessage("Country cannot be empty.");

            RuleFor(client => client.ContactName)
                .NotNull().WithMessage("Contact Name is required.")
                .NotEmpty().WithMessage("Contact Name cannot be empty.");

            RuleFor(client => client.ContactPhone)
                .NotNull().WithMessage("Contact Phone is required.")
                .NotEmpty().WithMessage("Contact Phone cannot be empty.")
                .MustAsync(async (phone, cancellationToken) => await IsUniquePhone(phone))
                .WithMessage("Contact Phone must be unique.");

            RuleFor(client => client.ContactEmail)
                .NotNull().WithMessage("Contact Email is required.")
                .NotEmpty().WithMessage("Contact Email cannot be empty.")
                .EmailAddress().WithMessage("Contact Email must be a valid email address.")
                .MustAsync(async (email, cancellationToken) => await IsUniqueEmail(email))
                .WithMessage("Contact Email must be unique.");
        }

        private async Task<bool> IsUniquePhone(string phone)
        {
            return !await _dbContext.Set<Client>().AnyAsync(c => c.ContactPhone == phone);
        }

        private async Task<bool> IsUniqueEmail(string email)
        {
            return !await _dbContext.Set<Client>().AnyAsync(c => c.ContactEmail == email);
        }
    }
}
