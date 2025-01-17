using System.Net;
using Backend.Features.Contacts;
using Backend.Features.Warehouses;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class WarehousesControllerTest
    {
        [Fact]
        public async Task GetAllWarehousesOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");
            // Check out options TODO

            // Act
            var result = await client.GetAsync("http://localhost:5031/api/warehouses");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetWarehouseByIdOnSuccessReturns204()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");
            // Check out options TODO

            // Act
            var result = await client.GetAsync("http://localhost:5031/api/warehouses/1");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddWarehouseOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");


            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Main Warehouse",
                Address = "123 Main St",
                Zip = "12345",
                City = "Metropolis",
                Province = "Central",
                Country = "Countryland",
                Contacts = new Contact[]
                {
                    new Contact { Id = 1, ContactName = "John Doe", ContactEmail = "john.doe@example.com", ContactPhone = "123-456-7890" }
                },
                CreatedAt = DateTime.Now
            };

            var content = JsonContent.Create(warehouse);
            // Check out options TODO

            // Act
            var result = await client.PostAsync("http://localhost:5031/api/warehouses", content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateWarehouseOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Main Warehouse",
                Address = "123 Main St",
                Zip = "12345",
                City = "Metropolis",
                Province = "Central",
                Country = "Countryland",
                Contacts = new Contact[]
                {
                    new Contact { Id = 1, ContactName = "John Doe", ContactEmail = "", ContactPhone = "123-456-7890" }
                },
                CreatedAt = DateTime.Now,
                UpdatedAt = null

            };

            var content = JsonContent.Create(warehouse);
            // Check out options TODO

            // Act
            var result = await client.PutAsync("http://localhost:5031/api/warehouses", content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
