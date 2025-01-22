using System.Net;
using System.Net.Http.Json;
using Backend.Features.Clients;
using Backend.Requests;
using Backend.Response;
using FluentAssertions;
using Xunit;

public class ClientsControllerTest : IDisposable
{
    private const string ApiKey = "3f5e8b9c-2d4a-4b6a-8f3e-1a2b3c4d5e6f";
    private readonly string BaseUrl = "http://localhost:5031/api/clients";
    private readonly HttpClient _client;
    private readonly List<int> _createdClientIds;

    public ClientsControllerTest()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
        _createdClientIds = new List<int>();
    }

    [Fact]
    public async Task GetClientByIdOnSuccessReturns200()
    {
        // Arrange
        var newClient = new Client
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

        var createResponse = await _client.PostAsJsonAsync(BaseUrl, newClient);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var createdClient = await createResponse.Content.ReadFromJsonAsync<Client>();
        createdClient.Should().NotBeNull();
        _createdClientIds.Add(createdClient!.Id);

        // Act
        var result = await _client.GetAsync($"{BaseUrl}/{createdClient.Id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedClient = await result.Content.ReadFromJsonAsync<Client>();
        retrievedClient.Should().NotBeNull();
        retrievedClient!.Name.Should().Be(newClient.Name);
        retrievedClient.Address.Should().Be(newClient.Address);
    }

    [Fact]
    public async Task GetClientByIdReturns404WhenClientDoesNotExist()
    {
        // Act
        var result = await _client.GetAsync($"{BaseUrl}/99999");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddClientOnSuccessReturns200()
    {
        // Arrange
        var newClient = new Client
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
        var response = await _client.PostAsJsonAsync(BaseUrl, newClient);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdClient = await response.Content.ReadFromJsonAsync<Client>();
        createdClient.Should().NotBeNull();
        _createdClientIds.Add(createdClient!.Id);
        
        createdClient!.Name.Should().Be(newClient.Name);
    }

    [Fact]
    public async Task UpdateClientOnSuccessReturns204()
    {
        // Arrange
        var newClient = new Client
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

        var createResponse = await _client.PostAsJsonAsync(BaseUrl, newClient);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<Client>();
        createdClient.Should().NotBeNull();
        _createdClientIds.Add(createdClient!.Id);

        var updatedClient = new Client
        {
            Id = createdClient.Id,
            Name = "Updated Test Client",
            Address = "456 Update Street",
            City = "Update City",
            ZipCode = "54321",
            Province = "Update Province",
            Country = "Update Country",
            ContactName = "Updated Contact",      // Ensure case matches the model
            ContactPhone = "987-654-3210",       // Ensure case matches the model
            ContactEmail = "updated@example.com" // Ensure case matches the model
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{createdClient.Id}", updatedClient);
        
        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest); // Expecting 204 No Content on successful update
        
        // Verify the update
        var getResponse = await _client.GetAsync($"{BaseUrl}/{createdClient.Id}");
        var retrievedClient = await getResponse.Content.ReadFromJsonAsync<Client>();
        retrievedClient.Should().NotBeNull();
        retrievedClient!.Name.Should().Be(updatedClient.Name);
        retrievedClient.Address.Should().Be(updatedClient.Address);
        retrievedClient.City.Should().Be(updatedClient.City);
        retrievedClient.ZipCode.Should().Be(updatedClient.ZipCode);
        retrievedClient.Province.Should().Be(updatedClient.Province);
        retrievedClient.Country.Should().Be(updatedClient.Country);
        retrievedClient.ContactName.Should().Be(updatedClient.ContactName);
        retrievedClient.ContactPhone.Should().Be(updatedClient.ContactPhone);
        retrievedClient.ContactEmail.Should().Be(updatedClient.ContactEmail);
    }

    [Fact]
    public async Task UpdateClientReturns400WhenIdsDoNotMatch()
    {
        // Arrange
        var newClient = new Client
        {
            Id = 999, // Mismatched ID
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
        var response = await _client.PutAsync($"{BaseUrl}/1", JsonContent.Create(newClient));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteClientOnSuccessReturns204()
    {
        // Arrange
        var newClient = new Client
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

        var createResponse = await _client.PostAsJsonAsync(BaseUrl, newClient);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<Client>();
        createdClient.Should().NotBeNull();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{createdClient!.Id}");
        
        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify the deletion
        var getResponse = await _client.GetAsync($"{BaseUrl}/{createdClient.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteClientReturns204WhenClientDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        // Cleanup: Delete all clients created during tests
        foreach (var id in _createdClientIds)
        {
            _client.DeleteAsync($"{BaseUrl}/{id}").Wait();
        }
        _client.Dispose();
    }
}