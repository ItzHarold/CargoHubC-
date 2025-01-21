using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Clients;
using Backend.Infrastructure.Database;
using Backend.Requests;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Backend.Features.Clients.Tests
{
    public class ClientServiceTests
    {
        private readonly ClientService _clientService;
        private readonly Mock<IValidator<Client>> _mockValidator;
        private readonly CargoHubDbContext _mockContext;

        public ClientServiceTests()
        {
            _mockValidator = new Mock<IValidator<Client>>();
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();

            // Set up a default behavior for the validator mock
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Client>(), default))
                .ReturnsAsync(new ValidationResult());

            _clientService = new ClientService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllClients_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _clientService.GetAllClients(null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddClient_ValidClient_IncreasesClientCount()
        {
            // Arrange
            var clientRequest = new ClientRequest
            {
                Name = "Test Client",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "test@test.com"
            };

            // Act
            var newClientId = await _clientService.AddClient(clientRequest);
            var allClients = _clientService.GetAllClients(null, null, null, null, null);

            // Assert
            Assert.Single(allClients);
            Assert.Contains(allClients, c => c.Id == newClientId && c.Name == clientRequest.Name);
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
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "test@test.com"
            };

            _mockContext.Clients?.Add(client);
            _mockContext.SaveChanges();

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
        public async Task UpdateClient_ClientExists_UpdatesClientData()
        {
            // Arrange
            var client = new Client
            {
                Id = 1,
                Name = "Original Name",
                Address = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Province = "Test Province",
                Country = "Test Country",
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "test@test.com"
            };

            _mockContext.Clients?.Add(client);
            _mockContext.SaveChanges();

            var updatedClient = new Client
            {
                Id = client.Id,
                Name = "Updated Name",
                Address = "456 Updated St",
                City = "Updated City",
                ZipCode = "54321",
                Province = "Updated Province",
                Country = "Updated Country",
                ContactName = "Jane Smith",
                ContactPhone = "123-4567",
                ContactEmail = "updated@test.com"
            };

            // Act
            _mockContext.Entry(client).State = EntityState.Detached;
            _mockContext.Clients?.Update(updatedClient);
            await _mockContext.SaveChangesAsync();

            var retrievedClient = _clientService.GetClientById(client.Id);

            // Assert
            Assert.NotNull(retrievedClient);
            Assert.Equal("Updated Name", retrievedClient?.Name);
            Assert.Equal("Updated City", retrievedClient?.City);
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
                ContactEmail = "test@test.com"
            };

            _mockContext.Clients?.Add(client);
            _mockContext.SaveChanges();

            // Act
            _clientService.DeleteClient(client.Id);
            var result = _clientService.GetAllClients(null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteClient_ClientDoesNotExist_NoChangesMade()
        {
            // Act
            _clientService.DeleteClient(999);

            // Assert
            Assert.Empty(_clientService.GetAllClients(null, null, null, null, null));
        }
    }
}
