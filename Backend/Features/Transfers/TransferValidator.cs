using FluentValidation;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Transfers
{
    public class TransferValidator : AbstractValidator<Transfer>
    {
        private readonly CargoHubDbContext _dbContext;

        public TransferValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(transfer => transfer.Reference)
                .NotNull().WithMessage("Reference is required.")
                .NotEmpty().WithMessage("Reference cannot be empty.");

            RuleFor(transfer => transfer.TransferFromLocationId)
                .NotNull().When(transfer => transfer.TransferToLocationId == null).WithMessage("Transfer To is required if Transfer From is specified.")
                .NotEmpty().When(transfer => transfer.TransferToLocationId == null).WithMessage("Transfer To is required if Transfer From is specified.");

            RuleFor(transfer => transfer.TransferToLocationId)
                .NotNull().When(transfer => transfer.TransferFromLocationId == null).WithMessage("Transfer From is required if Transfer To is specified.")
                .NotEmpty().When(transfer => transfer.TransferFromLocationId == null).WithMessage("Transfer From is required if Transfer To is specified.");

            RuleFor(transfer => transfer.TransferStatus)
                .NotNull().WithMessage("Transfer Status is required.")
                .NotEmpty().WithMessage("Transfer Status cannot be empty.");

            RuleForEach(transfer => transfer.TransferItems)
                .NotNull().WithMessage("Each transfer item must be valid.")
                .MustAsync(async (item, cancellationToken) => await ItemExists(item.ItemUid))
                .WithMessage("Item with UID {PropertyValue} not found.");

            RuleFor(transfer => transfer.TransferItems)
                .NotEmpty().WithMessage("At least one transfer item is required.");
        }
        private async Task<bool> ItemExists(string itemUid)
        {
            // Check if the Items DbSet is not null and perform the query
            if (_dbContext.Items != null)
            {
                var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Uid == itemUid);
                return item != null;
            }
            
            // If _dbContext.Items is null, return false
            return false;
        }
    }
}
