using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;

namespace Backend.Features.Contacts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts();
        Contact? GetContactById(int id);
        void AddContact(Contact contact);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
    }

    public class ContactService : IContactService
    {
        private readonly CargoHubDbContext _dbContext;

        public ContactService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Contact> GetAllContacts()
        {
            if (_dbContext.Contacts != null)
            {
                return _dbContext.Contacts.ToList();
            }
            return new List<Contact>();
        }

        public Contact? GetContactById(int id)
        {
            return _dbContext.Contacts?.Find(id);
        }

        public void AddContact(Contact contact)
        {
            _dbContext.Contacts?.Add(contact);
            _dbContext.SaveChanges();
        }
        public void UpdateContact(Contact contact)
        {
            _dbContext.Contacts?.Update(contact);
            _dbContext.SaveChanges();
        }

        public void DeleteContact(int id)
        {
            var contact = _dbContext.Contacts?.Find(id);
            if (contact != null)
            {
                _dbContext.Contacts?.Remove(contact);
                _dbContext.SaveChanges();
            }
        }
    }
}
