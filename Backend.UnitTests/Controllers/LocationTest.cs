using Backend.Features.Locations;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Locations.Tests
{
    public class LocationServiceTests
    {
        private readonly LocationService _locationService;

        public LocationServiceTests()
        {
            _locationService = new LocationService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllLocations_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _locationService.GetAllLocations();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddLocation_ValidLocation_IncreasesLocationCount()
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

            // Act
            _locationService.AddLocation(location);
            var allLocations = _locationService.GetAllLocations();

            // Assert
            Assert.Single(allLocations);
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
            _locationService.AddLocation(location);

            // Act
            var retrievedLocation = _locationService.GetLocationById(location.Id);

            // Assert
            Assert.NotNull(retrievedLocation);
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
        public void UpdateLocation_LocationExists_UpdatesLocationData()
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
            _locationService.AddLocation(location);

            var updatedLocation = new Location
            {
                Id = location.Id,
                WarehouseId = location.WarehouseId,
                Code = "Updated Code",
                Row = location.Row,
                Rack = location.Rack,
                Shelf = location.Shelf
            };

            // Act
            _locationService.UpdateLocation(updatedLocation);
            var retrievedLocation = _locationService.GetLocationById(location.Id);

            // Assert
            Assert.NotNull(retrievedLocation);
            Assert.Equal(updatedLocation.Code, retrievedLocation?.Code);
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
            _locationService.AddLocation(location);

            // Act
            _locationService.DeleteLocation(location.Id);
            var result = _locationService.GetAllLocations();

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
            _locationService.AddLocation(location);

            // Act
            _locationService.DeleteLocation(999);

            // Assert
            Assert.Single(_locationService.GetAllLocations());
        }
    }
}
