using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Locations;
using Backend.Infrastructure.Database;
using Backend.Request;
using Backend.UnitTests.Factories;
using FluentValidation;
using Xunit;

namespace Backend.Features.Locations.Tests
{
    public class LocationServiceTests
    {
        private readonly LocationService _locationService;
        private readonly CargoHubDbContext _mockContext;

        public LocationServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _locationService = new LocationService(_mockContext, new LocationValidator(_mockContext));
        }

        [Fact]
        public void GetAllLocations_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _locationService.GetAllLocations(null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddLocation_ValidLocation_IncreasesLocationCount()
        {
            // Arrange
            var locationRequest = new LocationRequest
            {
                WarehouseId = 1,
                Code = "3",
                Name = "Row: 1, Rack: 1, Shelf: A"
            };

            // Act
            await _locationService.AddLocation(locationRequest);
            var allLocations = _locationService.GetAllLocations(null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allLocations);
            Assert.Contains(allLocations, l => l.Code == locationRequest.Code);
        }

        [Fact]
        public void GetLocationById_LocationExists_ReturnsLocation()
        {
            // Arrange
            var location = new Location
            {
                Id = 1,
                WarehouseId = 1,
                Code = "3",
                Row = "1",
                Rack = "1",
                Shelf = "A"
            };
            _mockContext.Locations?.Add(location);
            _mockContext.SaveChanges();

            // Act
            var retrievedLocation = _locationService.GetLocationById(location.Id);

            // Assert
            Assert.NotNull(retrievedLocation);
            Assert.Equal(location.Code, retrievedLocation?.Code);
        }

        [Fact]
        public void GetLocationById_LocationDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _locationService.GetLocationById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateLocation_LocationExists_UpdatesLocationData()

        {
            // Arrange
            var location = new Location
            {
                Id = 1,
                WarehouseId = 1,
                Code = "3",
                Row = "1",
                Rack = "1",
                Shelf = "A"
            };
            _mockContext.Locations?.Add(location);
            _mockContext.SaveChanges();

            var updatedRequest = new LocationRequest
            {
                WarehouseId = location.WarehouseId,
                Code = "Updated Code",
                Name = "Row: 1, Rack: 1, Shelf: A"
            };

            // Act
            await _locationService.UpdateLocation(location.Id, updatedRequest);
            var retrievedLocation = _locationService.GetLocationById(location.Id);

            // Assert
            Assert.NotNull(retrievedLocation);
            Assert.Equal(updatedRequest.Code, retrievedLocation?.Code);
        }

        [Fact]
        public void DeleteLocation_LocationExists_RemovesLocation()
        {
            // Arrange
            var location = new Location
            {
                Id = 3,
                WarehouseId = 1,
                Code = "12345",
                Row = "1",
                Rack = "1",
                Shelf = "A"
            };
            _mockContext.Locations?.Add(location);
            _mockContext.SaveChanges();

            // Act
            _locationService.DeleteLocation(location.Id);
            var result = _locationService.GetAllLocations(null, null, null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteLocation_LocationDoesNotExist_NoChangesMade()
        {
            // Arrange
            var location = new Location
            {
                Id = 3,
                WarehouseId = 1,
                Code = "12345",
                Row = "1",
                Rack = "1",
                Shelf = "A"
            };
            _mockContext.Locations?.Add(location);
            _mockContext.SaveChanges();

            // Act
            _locationService.DeleteLocation(999);

            // Assert
            Assert.Single(_locationService.GetAllLocations(null, null, null, null, null, null, null));

        }
    }
}
