using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.Contacts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts(string? sort, string? direction, string? name, string? phone, string? email);
        Contact? GetContactById(int id);
        Task AddContact(Contact contact);
        Task UpdateContact(Contact contact);
        void DeleteContact(int id);
    }

    public class ContactService : IContactService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Contact> _validator;

        public ContactService(CargoHubDbContext dbContext, IValidator<Contact> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Contact> GetAllContacts(string? sort, string? direction, string? name, string? phone, string? email)
        {
            // Check if _dbContext.Contacts is null
            if (_dbContext.Contacts == null)
            {
                return new List<Contact>();
            }

            IQueryable<Contact> query = _dbContext.Contacts.AsQueryable();

            // Apply filtering based on the query parameters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.ContactName.Contains(name));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(c => c.ContactPhone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(c => c.ContactEmail.Contains(email));
            }

            // Apply sorting based on the query parameters
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "name":
                        query = direction == "desc" ? query.OrderByDescending(c => c.ContactName) : query.OrderBy(c => c.ContactName);
                        break;
                    case "phone":
                        query = direction == "desc" ? query.OrderByDescending(c => c.ContactPhone) : query.OrderBy(c => c.ContactPhone);
                        break;
                    case "email":
                        query = direction == "desc" ? query.OrderByDescending(c => c.ContactEmail) : query.OrderBy(c => c.ContactEmail);
                        break;
                    default:
                        // Default sorting behavior (e.g., by name)
                        query = query.OrderBy(c => c.ContactName);
                        break;
                }
            }

            // Return the filtered and sorted result as a list
            return query.ToList();
        }


        public Contact? GetContactById(int id)
        {
            return _dbContext.Contacts?.Find(id);
        }

        public async Task AddContact(Contact contact)
        {
            //

            _dbContext.Contacts?.Add(contact);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateContact(Contact contact)
        {
            _dbContext.Contacts?.Update(contact);
            await _dbContext.SaveChangesAsync();
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
