using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.Locations
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAllLocations();
        Location? GetLocationById(int id);
        Task AddLocation(IncomingLocation incomingLocation);
        Task UpdateLocation(IncomingLocation incomingLocation);
        void DeleteLocation(int id);
    }

    public class LocationService : ILocationService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Location> _validator;

        public LocationService(CargoHubDbContext dbContext, IValidator<Location> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Location> GetAllLocations()
        {
            if (_dbContext.Locations != null)
            {
                return _dbContext.Locations.ToList();
            }
            return new List<Location>();
        }

        public Location? GetLocationById(int id)
        {
            return _dbContext.Locations?.FirstOrDefault(l => l.Id == id);
        }

        public async Task AddLocation(IncomingLocation incomingLocation)
        {
            var location = TransformIncomingToLocation(incomingLocation);

            // Delegate validation to the validator
            var validationResult = await _validator.ValidateAsync(location);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            location.CreatedAt = DateTime.Now;
            _dbContext.Locations?.Add(location);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLocation(IncomingLocation incomingLocation)
        {
            var location = TransformIncomingToLocation(incomingLocation);

            // Delegate validation to the validator
            var validationResult = await _validator.ValidateAsync(location);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            location.UpdatedAt = DateTime.Now;
            _dbContext.Locations?.Update(location);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteLocation(int id)
        {
            var location = _dbContext.Locations?.FirstOrDefault(l => l.Id == id);
            if (location != null)
            {
                _dbContext.Locations?.Remove(location);
                _dbContext.SaveChanges();
            }
        }

        private static readonly string[] Delimiters = new[] { ", " };

        private static Location TransformIncomingToLocation(IncomingLocation incomingLocation)
        {
            var parts = incomingLocation.Name?.Split(Delimiters, StringSplitOptions.None) ?? Array.Empty<string>();

            string GetValue(string? part, string prefix)
            {
                if (string.IsNullOrEmpty(part)) return string.Empty;

                return part.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    ? part.Substring(prefix.Length).Trim()
                    : string.Empty;
            }

            return new Location
            {
                Id = incomingLocation.Id,
                WarehouseId = incomingLocation.WarehouseId,
                Code = incomingLocation.Code,
                Row = GetValue(parts.ElementAtOrDefault(0), "Row:"),
                Rack = GetValue(parts.ElementAtOrDefault(1), "Rack:"),
                Shelf = GetValue(parts.ElementAtOrDefault(2), "Shelf:"),
                CreatedAt = incomingLocation.CreatedAt,
                UpdatedAt = incomingLocation.UpdatedAt
            };
        }
    }
}
