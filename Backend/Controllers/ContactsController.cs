using System.Collections.Generic;
using Backend.Features.Contacts;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Contacts
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Contact>> GetAllContacts(
            [FromQuery] Dictionary<string, string?>? filters = null, 
            [FromQuery] string? sortBy = null, 
            [FromQuery] bool sortDescending = false)
        {
            var contact = _contactService.GetAllContacts(filters, sortBy, sortDescending);
            return Ok(contact);
        }

        [HttpGet("{id}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _contactService.GetContactById(id);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        public IActionResult AddContact(Contact contact)
        {
            _contactService.AddContact(contact);
            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }
            _contactService.UpdateContact(contact);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            _contactService.DeleteContact(id);
            return NoContent();
        }
    }
}
