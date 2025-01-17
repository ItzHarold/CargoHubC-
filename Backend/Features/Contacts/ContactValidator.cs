using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Contacts
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        private readonly CargoHubDbContext _dbContext;

        public ContactValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(contact => contact.ContactName)
                .NotNull().WithMessage("Contact Name is required.")
                .NotEmpty().WithMessage("Contact Name cannot be empty.");

            RuleFor(contact => contact.ContactPhone)
                .NotNull().WithMessage("Contact Phone is required.")
                .NotEmpty().WithMessage("Contact Phone cannot be empty.")
                .MustAsync(async (phone, cancellationToken) => await IsUniquePhone(phone))
                .WithMessage("Contact Phone must be unique.");

            RuleFor(contact => contact.ContactEmail)
                .NotNull().WithMessage("Contact Email is required.")
                .NotEmpty().WithMessage("Contact Email cannot be empty.")
                .EmailAddress().WithMessage("Contact Email must be a valid email address.")
                .MustAsync(async (email, cancellationToken) => await IsUniqueEmail(email))
                .WithMessage("Contact Email must be unique.");
        }

        private async Task<bool> IsUniquePhone(string phone)
        {
            return !await _dbContext.Set<Contact>().AnyAsync(c => c.ContactPhone == phone);
        }

        private async Task<bool> IsUniqueEmail(string email)
        {
            return !await _dbContext.Set<Contact>().AnyAsync(c => c.ContactEmail == email);
        }
    }
}
