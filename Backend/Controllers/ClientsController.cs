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
        public ActionResult<IEnumerable<Client>> GetAllClients(
            [FromQuery] Dictionary<string, string?>? filters = null, 
            [FromQuery] string? sortBy = null, 
            [FromQuery] bool sortDescending = false)
        {
            var clients = _clientService.GetAllClients(filters, sortBy, sortDescending);
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
    }
}
