using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

namespace Backend.Features.Clients
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients();
        Client? GetClientById(int id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
        IEnumerable<Client> GetClientFilteredAndSorted(
            string? name = null,
            string? address = null,
            string? city = null,
            string? zipCode = null,
            string? province = null,
            string? country = null,
            string? contactName = null,
            string? contactPhone = null,
            string? contactEmail = null,
            string? sortBy = null,
            bool sortDescending = false
        );
    
    }

    public class ClientService : IClientService
    {
        private readonly List<Client> _clients = new();

        public IEnumerable<Client> GetAllClients()
        {
            return _clients;
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

        public IEnumerable<Client> GetClientFilteredAndSorted(
            string? name = null,
            string? address = null,
            string? city = null,
            string? zipCode = null,
            string? province = null,
            string? country = null,
            string? contactName = null,
            string? contactPhone = null,
            string? contactEmail = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            // Filtering
            var query = _clients.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => c.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(address))
                query = query.Where(c => c.Address.Contains(address, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(city))
                query = query.Where(c => c.City.Contains(city, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(zipCode))
                query = query.Where(c => c.ZipCode.Contains(zipCode, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(province))
                query = query.Where(c => c.Province.Contains(province, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(country))
                query = query.Where(c => c.Country.Contains(country, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(contactName))
                query = query.Where(c => c.ContactName.Contains(contactName, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(contactPhone))
                query = query.Where(c => c.ContactPhone.Contains(contactPhone, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(contactEmail))
                query = query.Where(c => c.ContactEmail.Contains(contactEmail, StringComparison.InvariantCultureIgnoreCase));

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.ToLower(CultureInfo.InvariantCulture);
                query = sortBy switch
                {
                    "name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "address" => sortDescending ? query.OrderByDescending(c => c.Address) : query.OrderBy(c => c.Address),
                    "city" => sortDescending ? query.OrderByDescending(c => c.City) : query.OrderBy(c => c.City),
                    "zipcode" => sortDescending ? query.OrderByDescending(c => c.ZipCode) : query.OrderBy(c => c.ZipCode),
                    "province" => sortDescending ? query.OrderByDescending(c => c.Province) : query.OrderBy(c => c.Province),
                    "country" => sortDescending ? query.OrderByDescending(c => c.Country) : query.OrderBy(c => c.Country),
                    "contactname" => sortDescending ? query.OrderByDescending(c => c.ContactName) : query.OrderBy(c => c.ContactName),
                    "contactphone" => sortDescending ? query.OrderByDescending(c => c.ContactPhone) : query.OrderBy(c => c.ContactPhone),
                    "contactemail" => sortDescending ? query.OrderByDescending(c => c.ContactEmail) : query.OrderBy(c => c.ContactEmail),
                    _ => query
                };
            }
            return query.ToList();
        }
    }
}
