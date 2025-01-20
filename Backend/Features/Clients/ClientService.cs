using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using Backend.Response;
using Backend.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Clients
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients(string? sort, string? direction, string? name, string? city, string? country);
        Client? GetClientById(int id);
        Task<int> AddClient(ClientRequest clientRequest);
        Task UpdateClient(Client client);
        void DeleteClient(int id);
    }

    public class ClientService : IClientService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Client> _validator;

        public ClientService(CargoHubDbContext dbContext, IValidator<Client> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Client> GetAllClients(string? sort, string? direction, string? name, string? city, string? country)
        {
            // Check if _dbContext.Clients is null
            if (_dbContext.Clients == null)
            {
                // Return an empty list if Clients is null
                return new List<Client>();
            }

            IQueryable<Client> query = _dbContext.Clients.AsQueryable();

            // Apply filtering based on the query parameters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(c => c.City.Contains(city));
            }
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(c => c.Country.Contains(country));
            }

            // Apply sorting based on the query parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "name":
                        query = direction == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                        break;
                    case "address":
                        query = direction == "desc" ? query.OrderByDescending(c => c.Address) : query.OrderBy(c => c.Address);
                        break;
                    case "city":
                        query = direction == "desc" ? query.OrderByDescending(c => c.City) : query.OrderBy(c => c.City);
                        break;
                    case "country":
                        query = direction == "desc" ? query.OrderByDescending(c => c.Country) : query.OrderBy(c => c.Country);
                        break;
                    // Add more cases as needed for other properties
                    default:
                        // Default sorting (you can choose any default behavior here)
                        query = query.OrderBy(c => c.Name);
                        break;
                }
            }

            // Return the filtered and sorted result as a list
            return query.ToList();
        }
        
        public Client? GetClientById(int id)
        {
            return _dbContext.Clients?.Find(id);
        }

        public async Task<int> AddClient(ClientRequest clientRequest)
        {
            // var validationResult = await _validator.ValidateAsync(client);

            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            var client = new Client()
            {
                Name = clientRequest.Name,
                Address = clientRequest.Address,
                City = clientRequest.City,
                ZipCode = clientRequest.ZipCode,
                Province = clientRequest.Province,
                Country = clientRequest.Country,
                ContactName = clientRequest.ContactName,
                ContactPhone = clientRequest.ContactPhone,
                ContactEmail = clientRequest.ContactEmail
            };

            client.CreatedAt = DateTime.Now;
            client.UpdatedAt = client.CreatedAt;

            _dbContext.Clients?.Add(client);
            await _dbContext.SaveChangesAsync();
            return client.Id;
        }

        public async Task UpdateClient(Client client)
        {
            var validationResult = await _validator.ValidateAsync(client);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            client.UpdatedAt = DateTime.Now;
            _dbContext.Clients?.Update(client);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteClient(int id)
        {
            var client = _dbContext.Clients?.Find(id);
            if (client != null)
            {
                _dbContext.Clients?.Remove(client);
                _dbContext.SaveChanges();
            }
        }
    }
}
