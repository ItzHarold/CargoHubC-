using System.Collections.Generic;
using Backend.Features.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Clients
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult GetAllClients()
        {
            var clients = _clientService.GetAllClients();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _clientService.GetClientById(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        public IActionResult AddClient(Client client)
        {
            _clientService.AddClient(client);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }
            _clientService.UpdateClient(client);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            _clientService.DeleteClient(id);
            return NoContent();
        }

        [HttpGet("filtered-sorted")]
        public IActionResult GetClientsFilteredAndSorted(
            [FromQuery] string? name = null,
            [FromQuery] string? address = null,
            [FromQuery] string? city = null,
            [FromQuery] string? zipCode = null,
            [FromQuery] string? province = null,
            [FromQuery] string? country = null,
            [FromQuery] string? contactName = null,
            [FromQuery] string? contactPhone = null,
            [FromQuery] string? contactEmail = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false
        )
        {
            var clients = _clientService.GetClientFilteredAndSorted(
                name, address, city, zipCode, province, country, contactName, contactPhone, contactEmail, sortBy, sortDescending
            );

            return Ok(clients);
        }
    }
}
