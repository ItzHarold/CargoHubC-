using FluentValidation;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Inventories
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        private readonly CargoHubDbContext _dbContext;

        public InventoryValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(inventory => inventory.ItemId)
                .NotNull().WithMessage("Item ID is required.");

            RuleFor(inventory => inventory.LocationId)
                .NotNull().WithMessage("Location ID array is required.")
                .MustAsync(async (locations, cancellationToken) => await AllLocationsExist(locations))
                .WithMessage("One or more Location IDs do not exist in the Locations table.");

            RuleFor(inventory => inventory.TotalOnHand)
                .GreaterThanOrEqualTo(0).WithMessage("Total On Hand must be zero or greater.");

            RuleFor(inventory => inventory.TotalExpected)
                .GreaterThanOrEqualTo(0).WithMessage("Total Expected must be zero or greater.");

            RuleFor(inventory => inventory.TotalOrdered)
                .GreaterThanOrEqualTo(0).WithMessage("Total Ordered must be zero or greater.");

            RuleFor(inventory => inventory.TotalAllocated)
                .GreaterThanOrEqualTo(0).WithMessage("Total Allocated must be zero or greater.");

            RuleFor(inventory => inventory.TotalAvailable)
                .GreaterThanOrEqualTo(0).WithMessage("Total Available must be zero or greater.");
        }

        private async Task<bool> AllLocationsExist(int[] locationIds)
        {
            foreach (var locationId in locationIds)
            {
                var exists = _dbContext.Locations != null && await _dbContext.Locations.AnyAsync(l => l.Id == locationId);
                if (!exists)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
