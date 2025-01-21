using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Features.Contacts;
using Backend.Infrastructure.Database;
using Backend.UnitTests.Factories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Backend.Features.Contacts.Tests
{
    public class ContactServiceTests
    {
        private readonly ContactService _contactService;
        private readonly Mock<IValidator<Contact>> _mockValidator;
        private readonly CargoHubDbContext _mockContext;

        public ContactServiceTests()
        {
            _mockValidator = new Mock<IValidator<Contact>>();
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();

            // Set up a default behavior for the validator mock
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Contact>(), default))
                .ReturnsAsync(new ValidationResult());

            _contactService = new ContactService(_mockContext, _mockValidator.Object);
        }

        [Fact]
        public void GetAllContacts_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _contactService.GetAllContacts(null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddContact_ValidContact_IncreasesContactCount()
        {
            // Arrange
            var contact = new Contact
            {
                ContactName = "John Doe",
                ContactPhone = "555-1234",
                ContactEmail = "john.doe@test.com"
            };

            // Act
            await _contactService.AddContact(contact);
            var allContacts = _contactService.GetAllContacts(null, null, null, null, null);

            // Assert
            Assert.Single(allContacts);
            Assert.Contains(allContacts, c => c.ContactName == contact.ContactName);
        }

        [Fact]
        public void GetContactById_ContactExists_ReturnsContact()
        {
            // Arrange
            var contact = new Contact
            {
                ContactName = "Jane Smith",
                ContactPhone = "555-5678",
                ContactEmail = "jane.smith@test.com"
            };

            _mockContext.Contacts?.Add(contact);
            _mockContext.SaveChanges();

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
        public async Task UpdateContact_ContactExists_UpdatesContactData()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Original Name",
                ContactPhone = "555-1234",
                ContactEmail = "original@test.com"
            };

            // Add the initial Contact to the mock context
            _mockContext.Contacts?.Add(contact);
            _mockContext.SaveChanges();

            // Retrieve the contact to ensure it is tracked by the DbContext
            var existingContact = _mockContext.Contacts?.First(c => c.Id == contact.Id);

            // Modify the existing entity directly
            if (existingContact != null)
            {
                existingContact.ContactName = "Updated Name";
                existingContact.ContactPhone = "555-5678";
                existingContact.ContactEmail = "updated@test.com";
            }

            // Act
            await _contactService.UpdateContact(existingContact!);

            // Retrieve the updated Contact from the service
            var retrievedContact = _contactService.GetContactById(contact.Id);

            // Assert
            Assert.NotNull(retrievedContact);
            Assert.Equal("Updated Name", retrievedContact?.ContactName);
            Assert.Equal("555-5678", retrievedContact?.ContactPhone);
            Assert.Equal("updated@test.com", retrievedContact?.ContactEmail);
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
                ContactEmail = "nonexistent@test.com"
            };

            // Act
            _contactService.UpdateContact(updatedContact);

            // Assert
            Assert.Empty(_contactService.GetAllContacts(null, null, null, null, null));
        }

        [Fact]
        public void DeleteContact_ContactExists_RemovesContact()
        {
            // Arrange
            var contact = new Contact
            {
                ContactName = "Test User",
                ContactPhone = "555-1234",
                ContactEmail = "test.user@test.com"
            };

            _mockContext.Contacts?.Add(contact);
            _mockContext.SaveChanges();

            // Act
            _contactService.DeleteContact(contact.Id);
            var result = _contactService.GetAllContacts(null, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteContact_ContactDoesNotExist_NoChangesMade()
        {
            // Act
            _contactService.DeleteContact(999);

            // Assert
            Assert.Empty(_contactService.GetAllContacts(null, null, null, null, null));
        }
    }
}
