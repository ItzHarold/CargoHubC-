using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

namespace Backend.Features.Contacts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts();
        Contact? GetContactById(int id);
        void AddContact(Contact contact);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
        IEnumerable<Contact> GetContactFilteredAndSorted(
        string? name = null,
        string? email = null,
        string? phoneNumber = null,
        string? sortBy = null,
        bool sortDescending = false
        );
    }

    public class ContactService : IContactService
    {
        private readonly List<Contact> _contacts = new();

        public IEnumerable<Contact> GetAllContacts()
        {
            return _contacts;
        }

        public Contact? GetContactById(int id)
        {
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public void AddContact(Contact contact)
        {
            contact.Id = _contacts.Count > 0 ? _contacts.Max(c => c.Id) + 1 : 1;
            _contacts.Add(contact);
        }
        public void UpdateContact(Contact contact)
        {
            var existingContact = GetContactById(contact.Id);
            if (existingContact != null)
            {
                var updatedContact = new Contact
                {
                    Id = existingContact.Id,
                    ContactName = contact.ContactName,
                    ContactPhone = contact.ContactPhone,
                    ContactEmail = contact.ContactEmail
                };
                _contacts[_contacts.IndexOf(existingContact)] = updatedContact;
            }
        }

        public void DeleteContact(int id)
        {
            var contact = GetContactById(id);
            if (contact != null)
            {
                _contacts.Remove(contact);
            }
        }
        public IEnumerable<Contact> GetContactFilteredAndSorted(
        string? name = null,
        string? email = null,
        string? phoneNumber = null,
        string? sortBy = null,
        bool sortDescending = false
        )
        {
            var query = _contacts.AsEnumerable();

            // Apply filters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(contact => contact.ContactName?.Contains(name, StringComparison.OrdinalIgnoreCase) == true);
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(contact => contact.ContactEmail?.Contains(email, StringComparison.OrdinalIgnoreCase) == true);
            }
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query = query.Where(contact => contact.ContactPhone?.Contains(phoneNumber, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Apply sorting
            query = sortBy?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "id" => sortDescending ? query.OrderByDescending(contact => contact.Id) : query.OrderBy(contact => contact.Id),
                "name" => sortDescending ? query.OrderByDescending(contact => contact.ContactName) : query.OrderBy(contact => contact.ContactName),
                "email" => sortDescending ? query.OrderByDescending(contact => contact.ContactEmail) : query.OrderBy(contact => contact.ContactEmail),
                "phonenumber" => sortDescending ? query.OrderByDescending(contact => contact.ContactPhone) : query.OrderBy(contact => contact.ContactPhone),
                _ => query
            };

            return query;
        }
    }
}
