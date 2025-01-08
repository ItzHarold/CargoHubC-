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
                }
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
                Name = "Updated Warehouse",
                Address = "123 Main St",
                Zip = "12345",
                City = "Metropolis",
                Province = "Central",
                Country = "Countryland",
                Contacts = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        ContactName = "John Doe Updated",
                        ContactEmail = "john.doe@example.com",
                        ContactPhone = "123-456-7890"
                    }
                }
            };

            var content = JsonContent.Create(warehouse);
            // Check out options TODO

            // Act
            var result = await client.PutAsync("http://localhost:5031/api/warehouses", content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteWarehouseOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            // First, we need to ensure the warehouse exists
            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Warehouse to Delete",
                Address = "123 Main St",
                Zip = "12345",
                City = "Metropolis",
                Province = "Central",
                Country = "Countryland",
                Contacts = new Contact[]
                {
                    new Contact { Id = 1, ContactName = "John Doe", ContactEmail = "john.doe@example.com", ContactPhone = "123-456-7890" }
                }
            };

            var content = JsonContent.Create(warehouse);
            // Act: Add the warehouse first
            var addResult = await client.PostAsync("http://localhost:5031/api/warehouses", content);
            addResult.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act: Now delete the warehouse with Id 1
            var deleteResult = await client.DeleteAsync("http://localhost:5031/api/warehouses/1");

            // Assert: The delete request should return a 200 OK status
            deleteResult.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // Act: Confirm that the warehouse is deleted by trying to fetch it
            var getResult = await client.GetAsync("http://localhost:5031/api/warehouses/1");

            // Assert: After deletion, we should get a 204 No Content or 404 Not Found status
            // Since the controller doesn't explicitly return 404 for deleted items,
            // 204 No Content is returned when the item doesn't exist.
            getResult.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

    }
}
