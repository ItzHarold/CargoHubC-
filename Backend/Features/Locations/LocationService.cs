using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Infrastructure.Database;
using Backend.Request;
using FluentValidation;

namespace Backend.Features.Locations
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAllLocations();
        Location? GetLocationById(int id);
        Task<Location> AddLocation(LocationRequest locationRequest);
        Task UpdateLocation(int id, LocationRequest locationRequest);
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

        public async Task<Location> AddLocation(LocationRequest locationRequest)
        {
            var (row, rack, shelf) = ParseName(locationRequest.Name);

            var location = new Location
            {
                WarehouseId = locationRequest.WarehouseId,
                Code = locationRequest.Code,
                Row = row,
                Rack = rack,
                Shelf = shelf,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.Locations?.Add(location);
            await _dbContext.SaveChangesAsync();

            return location;
        }

        public async Task UpdateLocation(int id, LocationRequest locationRequest)
        {
            var location = _dbContext.Locations?.FirstOrDefault(l => l.Id == id);
            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found.");
            }

            var (row, rack, shelf) = ParseName(locationRequest.Name);

            location.WarehouseId = locationRequest.WarehouseId;
            location.Code = locationRequest.Code;
            location.Row = row;
            location.Rack = rack;
            location.Shelf = shelf;
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

        private static (string Row, string Rack, string Shelf) ParseName(string name)
        {
            var delimiters = new[] { ", " };
            var parts = name.Split(delimiters, StringSplitOptions.None);

            string GetValue(string? part, string prefix)
            {
                if (string.IsNullOrEmpty(part)) return string.Empty;

                return part.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    ? part.Substring(prefix.Length).Trim()
                    : string.Empty;
            }

            return (
                Row: GetValue(parts.ElementAtOrDefault(0), "Row:"),
                Rack: GetValue(parts.ElementAtOrDefault(1), "Rack:"),
                Shelf: GetValue(parts.ElementAtOrDefault(2), "Shelf:")
            );
        }
    }
}
