using System.Collections.Generic;
using Backend.Features.Contacts;
using FluentValidation;
using FluentValidation.Results;
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
        public IActionResult GetAllContacts([FromQuery] string? sort, [FromQuery] string? direction, [FromQuery] string? name, [FromQuery] string? phone, [FromQuery] string? email)
        {
            var contacts = _contactService.GetAllContacts(sort, direction, name, phone, email);
            return Ok(contacts);
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

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] Contact contact)
        {
            try
            {
                await _contactService.AddContact(contact);
                return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest("Contact ID in the path does not match the ID in the body.");
            }

            try
            {
                await _contactService.UpdateContact(contact);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            _contactService.DeleteContact(id);
            return NoContent();
        }
    }
}
