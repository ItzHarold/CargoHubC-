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
        IEnumerable<Location> GetAllLocations(
            string? sort,
            string? direction,
            int? warehouseId,
            string? code,
            string? row,
            string? rack,
            string? shelf);
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

        public IEnumerable<Location> GetAllLocations(
            string? sort,
            string? direction,
            int? warehouseId,
            string? code,
            string? row,
            string? rack,
            string? shelf)
        {
            if (_dbContext.Locations == null)
            {
                return new List<Location>();
            }
            // Start with a base query for locations
            var query = _dbContext.Locations.AsQueryable();

            // Apply filtering based on the query parameters
            if (warehouseId.HasValue)
            {
                query = query.Where(l => l.WarehouseId == warehouseId);
            }

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(l => l.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(row))
            {
                query = query.Where(l => l.Row.Contains(row));
            }

            if (!string.IsNullOrEmpty(rack))
            {
                query = query.Where(l => l.Rack.Contains(rack));
            }

            if (!string.IsNullOrEmpty(shelf))
            {
                query = query.Where(l => l.Shelf.Contains(shelf));
            }

            // Apply sorting based on the sort and direction parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "warehouse_id":
                        query = direction == "desc" ? query.OrderByDescending(l => l.WarehouseId) : query.OrderBy(l => l.WarehouseId);
                        break;
                    case "code":
                        query = direction == "desc" ? query.OrderByDescending(l => l.Code) : query.OrderBy(l => l.Code);
                        break;
                    case "row":
                        query = direction == "desc" ? query.OrderByDescending(l => l.Row) : query.OrderBy(l => l.Row);
                        break;
                    case "rack":
                        query = direction == "desc" ? query.OrderByDescending(l => l.Rack) : query.OrderBy(l => l.Rack);
                        break;
                    case "shelf":
                        query = direction == "desc" ? query.OrderByDescending(l => l.Shelf) : query.OrderBy(l => l.Shelf);
                        break;
                    default:
                        query = query.OrderBy(l => l.Code); // Default sorting by `code`
                        break;
                }
            }

            // Return the filtered and sorted locations
            return query.ToList();
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
