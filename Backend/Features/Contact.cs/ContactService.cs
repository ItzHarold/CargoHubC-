using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;
using Backend.Features.FilterAndSort;

namespace Backend.Features.Contacts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts(Dictionary<string, string?>? filters = null, string? sortBy = null, bool sortDescending = false);
        Contact? GetContactById(int id);
        void AddContact(Contact contact);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
    }

    public class ContactService : IContactService
    {
        private readonly List<Contact> _contacts = new();

        public IEnumerable<Contact> GetAllContacts(
                Dictionary<string, string?>? filters = null, 
                string? sortBy = null,
                bool sortDescending = false)
        {
            return _contacts.AsEnumerable().FilterAndSort(filters, sortBy, sortDescending);
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
    }
}
