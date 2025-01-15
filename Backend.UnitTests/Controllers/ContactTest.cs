using System.Collections.Generic;
using System.Linq;
using Xunit;
using Backend.UnitTests.Factories;

namespace Backend.Features.Contacts.Tests
{
    public class ContactServiceTests
    {
        private readonly ContactService _contactService;

        public ContactServiceTests()
        {
            _contactService = new ContactService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllContacts_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _contactService.GetAllContacts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddContact_ValidContact_IncreasesContactCount()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Mr Test",
                ContactPhone = "555-1234",
                ContactEmail = "Test@example.com"
            };

            // Act
            _contactService.AddContact(contact);
            var allContacts = _contactService.GetAllContacts();

            // Assert
            Assert.Single(allContacts);
            Assert.Equal(contact.ContactName, allContacts.First().ContactName);
        }

        [Fact]
        public void GetContactById_ContactExists_ReturnsContact()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 2,
                ContactName = "Test Test",
                ContactPhone = "555-5678",
                ContactEmail = "Test@example.com"
            };
            _contactService.AddContact(contact);

            // Act
            var retrievedContact = _contactService.GetContactById(contact.Id);

            // Assert
            Assert.NotNull(retrievedContact);
            Assert.Equal(contact.ContactName, retrievedContact?.ContactName);
        }

        [Fact]
        public void GetContactById_ContactDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _contactService.GetContactById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateContact_ContactExists_UpdatesContactData()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Original Name",
                ContactPhone = "555-1234",
                ContactEmail = "original@example.com"
            };
            _contactService.AddContact(contact);

            var updatedContact = new Contact
            {
                Id = contact.Id,
                ContactName = "Updated Name",
                ContactPhone = "555-5678",
                ContactEmail = "updated@example.com"
            };

            // Act
            _contactService.UpdateContact(updatedContact);
            var retrievedContact = _contactService.GetContactById(contact.Id);

            // Assert
            Assert.NotNull(retrievedContact);
            Assert.Equal(updatedContact.ContactName, retrievedContact?.ContactName);
            Assert.Equal(updatedContact.ContactPhone, retrievedContact?.ContactPhone);
        }

        [Fact]
        public void UpdateContact_ContactDoesNotExist_NoChangesMade()
        {
            // Arrange
            var updatedContact = new Contact
            {
                Id = 999,
                ContactName = "Nonexistent Contact",
                ContactPhone = "555-5678",
                ContactEmail = "nonexistent@example.com"
            };

            // Act
            _contactService.UpdateContact(updatedContact);

            // Assert
            Assert.Empty(_contactService.GetAllContacts());
        }

        [Fact]
        public void DeleteContact_ContactExists_RemovesContact()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Ms Test",
                ContactPhone = "555-1234",
                ContactEmail = "Test@example.com"
            };
            _contactService.AddContact(contact);

            // Act
            _contactService.DeleteContact(contact.Id);
            var result = _contactService.GetAllContacts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteContact_ContactDoesNotExist_NoChangesMade()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Test test",
                ContactPhone = "555-5678",
                ContactEmail = "Test@example.com"
            };
            _contactService.AddContact(contact);

            // Act
            _contactService.DeleteContact(999);

            // Assert
            Assert.Single(_contactService.GetAllContacts());
        }
    }
}
