using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Features.FilterAndSort;

namespace Backend.Features.Clients
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients(Dictionary<string, string?>? filters = null, string? sortBy = null, bool sortDescending = false);
        Client? GetClientById(int id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
    }

    public class ClientService : IClientService
    {
        private readonly List<Client> _clients = new();

        public IEnumerable<Client> GetAllClients(
                Dictionary<string, string?>? filters = null, 
                string? sortBy = null,
                bool sortDescending = false)
        {
            return _clients.AsEnumerable().FilterAndSort(filters, sortBy, sortDescending);
        }


        public Client? GetClientById(int id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
        }

        public void AddClient(Client client)
        {
            client.Id = _clients.Count > 0 ? _clients.Max(c => c.Id) + 1 : 1;
            _clients.Add(client);
        }

        public void UpdateClient(Client client)
        {
            var existingClient = GetClientById(client.Id);
            if (existingClient != null)
            {
                var updatedClient = new Client
                {
                    Id = existingClient.Id,
                    Name = client.Name,
                    Address = client.Address,
                    City = client.City,
                    ZipCode = client.ZipCode,
                    Province = client.Province,
                    Country = client.Country,
                    ContactName = client.ContactName,
                    ContactPhone = client.ContactPhone,
                    ContactEmail = client.ContactEmail
                };
                _clients[_clients.IndexOf(existingClient)] = updatedClient;
            }
        }

        public void DeleteClient(int id)
        {
            var client = GetClientById(id);
            if (client != null)
            {
                _clients.Remove(client);
            }
        }
    }
}
