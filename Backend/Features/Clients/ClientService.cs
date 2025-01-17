using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Clients
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients();
        Client? GetClientById(int id);
        Task AddClient(Client client);
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

        public async Task AddClient(Client client)
        {
            var validationResult = await _validator.ValidateAsync(client);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            client.CreatedAt = DateTime.Now;
            _dbContext.Clients?.Add(client);
            await _dbContext.SaveChangesAsync();
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
