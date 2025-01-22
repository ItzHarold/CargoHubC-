using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Backend.Requests;

namespace Backend.IntegrationTests.Controllers
{
    public class ClientsControllerTest
    {
        private readonly string _baseUrl = "http://localhost:5031/api/clients";
        private readonly HttpClient _client;

        public ClientsControllerTest()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("X-API-KEY", "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f");
        }

        [Fact]
        public async Task GetAllClients_Returns200Ok()
        {
            // Act
            var response = await _client.GetAsync(_baseUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClientById_ValidId_Returns200Ok()
        {
            // Arrange
            var clientId = 1;

            // Act
            var response = await _client.GetAsync($"{_baseUrl}/{clientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClientById_InvalidId_Returns404NotFound()
        {
            // Arrange
            var invalidClientId = 9999;

            // Act
            var response = await _client.GetAsync($"{_baseUrl}/{invalidClientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddClient_ValidClient_Returns200Ok()
        {
            // Arrange
            var clientRequest = new ClientRequest
            {
                Name = "Test Client",
                Address = "123 Test Street",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "Test Contact",
                ContactPhone = "123-456-7890",
                ContactEmail = "test@example.com"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, clientRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddClient_InvalidClient_Returns400BadRequest()
        {
            // Arrange
            var invalidClientRequest = new ClientRequest
            {
                Name = "", // Invalid: Name cannot be empty
                Address = "", // Invalid: Address cannot be empty
                City = "", // Invalid: City cannot be empty
                ZipCode = "", // Invalid: ZipCode cannot be empty
                Province = "", // Invalid: Province cannot be empty
                Country = "", // Invalid: Country cannot be empty
                ContactName = "", // Invalid: ContactName cannot be empty
                ContactPhone = "", // Invalid: ContactPhone cannot be empty
                ContactEmail = "invalid-email-format" // Invalid: Incorrect email format
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrl, invalidClientRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task UpdateClient_ValidClient_Returns204NoContent()
        {
            // Arrange
            var clientId = 1;
            var clientUpdateRequest = new ClientRequest
            {
                Name = "Updated Client",
                Address = "456 Updated Street",
                City = "Updated City",
                ZipCode = "67890",
                Province = "Updated Province",
                Country = "Updated Country",
                ContactName = "Updated Contact",
                ContactPhone = "098-765-4321",
                ContactEmail = "updated@example.com"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/{clientId}", clientUpdateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateClient_InvalidClient_Returns400BadRequest()
        {
            // Arrange
            var clientId = 1;
            var invalidClientUpdateRequest = new ClientRequest
            {
                Name = "", // Invalid: Name cannot be empty
                Address = "", // Invalid: Address cannot be empty
                City = "", // Invalid: City cannot be empty
                ZipCode = "", // Invalid: ZipCode cannot be empty
                Province = "", // Invalid: Province cannot be empty
                Country = "", // Invalid: Country cannot be empty
                ContactName = "", // Invalid: ContactName cannot be empty
                ContactPhone = "", // Invalid: ContactPhone cannot be empty
                ContactEmail = "invalid-email-format" // Invalid: Incorrect email format
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrl}/{clientId}", invalidClientUpdateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task DeleteClient_ValidId_Returns204NoContent()
        {
            // Arrange
            var clientId = 1;

            // Act
            var response = await _client.DeleteAsync($"{_baseUrl}/{clientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteClient_InvalidId_Returns404NotFound()
        {
            // Arrange
            var invalidClientId = 9999;

            // Act
            var response = await _client.DeleteAsync($"{_baseUrl}/{invalidClientId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
