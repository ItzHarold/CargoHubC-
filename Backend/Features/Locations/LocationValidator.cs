using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Locations
{
    public class LocationValidator : AbstractValidator<Location>
    {
        private readonly CargoHubDbContext _dbContext;

        public LocationValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(location => location.WarehouseId)
                .GreaterThan(0).WithMessage("WarehouseId must be greater than 0.");

            RuleFor(location => location.Code)
                .NotEmpty().WithMessage("Code is required.");

            RuleFor(location => location.Row)
                .NotEmpty().WithMessage("Row is required.");

            RuleFor(location => location.Rack)
                .NotEmpty().WithMessage("Rack is required.");

            RuleFor(location => location.Shelf)
                .NotEmpty().WithMessage("Shelf is required.");

            RuleFor(location => location)
                .MustAsync(BeUniqueInWarehouse)
                .WithMessage("A location with the same Row, Rack, and Shelf already exists in this warehouse.");
        }

        private async Task<bool> BeUniqueInWarehouse(Location location, CancellationToken cancellationToken)
        {
            return _dbContext.Locations != null && !await _dbContext.Locations.AnyAsync(l =>
                l.WarehouseId == location.WarehouseId &&
                l.Row == location.Row &&
                l.Rack == location.Rack &&
                l.Shelf == location.Shelf &&
                l.Id != location.Id, cancellationToken);
        }
    }
}
