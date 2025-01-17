using System.Collections.Generic;
using System.Linq;
using Backend.Infrastructure.Database;
using FluentValidation;

namespace Backend.Features.Contacts
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts();
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

        public async Task AddContact(Contact contact)
        {
            //

            _dbContext.Contacts?.Add(contact);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateContact(Contact contact)
        {
            var validationResult = await _validator.ValidateAsync(contact);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
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
