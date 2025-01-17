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
        IEnumerable<Client> GetAllClients();
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

        public IEnumerable<Client> GetAllClients()
        {
            if (_dbContext.Clients != null)
            {
                return _dbContext.Clients.ToList();
            }
            return new List<Client>();
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
