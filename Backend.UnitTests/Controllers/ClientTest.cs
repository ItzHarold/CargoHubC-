using System.Collections.Generic;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Clients.Tests
{
    public class ClientServiceTests
    {
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _clientService = new ClientService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllClients_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _clientService.GetAllClients();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddClient_ValidClient_IncreasesClientCount()
        {
            // Arrange
            var client = new Client
            {
                Name = "Test Client",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "Test Test",
                ContactPhone = "555-1234",
                ContactEmail = "Test@test.com"
            };

            // Act
            _clientService.AddClient(client);
            var allClients = _clientService.GetAllClients();

            // Assert
            Assert.Single(allClients);
            Assert.Equal(client.Name, allClients.First().Name);
        }

        [Fact]
        public void GetClientById_ClientExists_ReturnsClient()
        {
            // Arrange
            var client = new Client
            {
                Name = "Test Client",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "Test test",
                ContactPhone = "123-1234",
                ContactEmail = "Test@test.com"
            };
            _clientService.AddClient(client);

            // Act
            var retrievedClient = _clientService.GetClientById(client.Id);

            // Assert
            Assert.NotNull(retrievedClient);
            Assert.Equal(client.Name, retrievedClient?.Name);
        }

        [Fact]
        public void GetClientById_ClientDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _clientService.GetClientById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateClient_ClientExists_UpdatesClientData()
        {
            // Arrange
            var client = new Client
            {
                Name = "Original Name",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "Test Test",
                ContactPhone = "123-1234",
                ContactEmail = "Test@test.com"
            };
            _clientService.AddClient(client);

            var updatedClient = new Client
            {
                Id = client.Id,
                Name = "Updated Name",
                Address = "123 Updated St",
                City = "Updated City",
                ZipCode = "54321",
                Province = "Updated Province",
                Country = "Updated Country",
                ContactName = "Jane Smith",
                ContactPhone = "123-45678",
                ContactEmail = "Test@test.com"
            };

            // Act
            _clientService.UpdateClient(updatedClient);
            var retrievedClient = _clientService.GetClientById(client.Id);

            // Assert
            Assert.NotNull(retrievedClient);
            Assert.Equal(updatedClient.Name, retrievedClient?.Name);
            Assert.Equal(updatedClient.City, retrievedClient?.City);
        }

        [Fact]
        public void UpdateClient_ClientDoesNotExist_NoChangesMade()
        {
            // Arrange
            var updatedClient = new Client
            {
                Id = 999,
                Name = "Nonexistent Client",
                Address = "123 Updated St",
                City = "Updated City",
                ZipCode = "54321",
                Province = "Updated Province",
                Country = "Updated Country",
                ContactName = "Sir Test",
                ContactPhone = "123-45678",
                ContactEmail = "Test@test.com"
            };

            // Act
            _clientService.UpdateClient(updatedClient);

            // Assert
            Assert.Empty(_clientService.GetAllClients());
        }

        [Fact]
        public void DeleteClient_ClientExists_RemovesClient()
        {
            // Arrange
            var client = new Client
            {
                Name = "Test Client",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "Test@test.com"
            };
            _clientService.AddClient(client);

            // Act
            _clientService.DeleteClient(client.Id);
            var result = _clientService.GetAllClients();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteClient_ClientDoesNotExist_NoChangesMade()
        {
            // Arrange
            var client = new Client
            {
                Name = "Test Client",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "Test@test.com"
            };
            _clientService.AddClient(client);

            // Act
            _clientService.DeleteClient(999);

            // Assert
            Assert.Single(_clientService.GetAllClients());
        }
    }
}
