using System.Net;
using Backend.Features.Clients;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class ClientsControllerTest
    {
        [Fact]
        public async Task GetAllClientsOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            // Act
            var result = await client.GetAsync("http://localhost:5031/api/clients");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClientByIdOnSuccessReturns200()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            // Add a client first
            var newClient = new Client
            {
                Name = "New Client",
                Address = "456 Oak St",
                City = "Uptown",
                ZipCode = "67890",
                Province = "Northern",
                Country = "Countryland",
                ContactName = "Jane Smith",
                ContactPhone = "987-654-3210",
                ContactEmail = "jane.smith@example.com"
            };

            var content = JsonContent.Create(newClient);
            var addResult = await client.PostAsync("http://localhost:5031/api/clients", content);
            
            // Ensure that client was successfully added (201 Created)
            addResult.StatusCode.Should().Be(HttpStatusCode.Created);

            // Extract the client ID from the response or check for the return content
            var createdClient = await addResult.Content.ReadFromJsonAsync<Client>();
            var createdClientId = createdClient?.Id;

            // Act: Now fetch the created client by its ID
            var getResult = await client.GetAsync($"http://localhost:5031/api/clients/{createdClientId}");

            // Assert: The get request should return a 200 OK status for the newly created client
            getResult.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task AddClientOnSuccessReturns201()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            var newClient = new Client
            {
                Name = "New Client",
                Address = "456 Oak St",
                City = "Uptown",
                ZipCode = "67890",
                Province = "Northern",
                Country = "Countryland",
                ContactName = "Jane Smith",
                ContactPhone = "987-654-3210",
                ContactEmail = "jane.smith@example.com"
            };

            var content = JsonContent.Create(newClient);

            // Act
            var result = await client.PostAsync("http://localhost:5031/api/clients", content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateClientOnSuccessReturns204()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            var updatedClient = new Client
            {
                Id = 1,
                Name = "Updated Client",
                Address = "123 Main St",
                City = "Downtown",
                ZipCode = "54321",
                Province = "Central",
                Country = "Countryland",
                ContactName = "John Doe",
                ContactPhone = "321-654-9870",
                ContactEmail = "john.doe@example.com"
            };

            var content = JsonContent.Create(updatedClient);

            // Act
            var result = await client.PutAsync("http://localhost:5031/api/clients/1", content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteClientOnSuccessReturns204()
        {
            // Arrange
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");

            // First, we need to ensure the client exists
            var newClient = new Client
            {
                Name = "Client to Delete",
                Address = "789 Pine St",
                City = "Lakeside",
                ZipCode = "11223",
                Province = "Eastern",
                Country = "Countryland",
                ContactName = "Jake White",
                ContactPhone = "654-321-9870",
                ContactEmail = "jake.white@example.com"
            };

            var content = JsonContent.Create(newClient);
            // Act: Add the client first
            var addResult = await client.PostAsync("http://localhost:5031/api/clients", content);
            addResult.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: Now delete the client with Id 1
            var deleteResult = await client.DeleteAsync("http://localhost:5031/api/clients/1");

            // Assert: The delete request should return a 204 No Content status
            deleteResult.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Act: Confirm that the client is deleted by trying to fetch it
            var getResult = await client.GetAsync("http://localhost:5031/api/clients/1");

            // Assert: After deletion, we should get a 404 Not Found status
            getResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
