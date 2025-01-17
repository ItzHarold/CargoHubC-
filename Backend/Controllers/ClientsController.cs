using System.Collections.Generic;
using Backend.Features.Clients;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Backend.Response;
using Backend.Requests;

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

            var response = new ClientResponse
            {
                Id = client.Id,
                Name = client.Name,
                Address = client.Address,
                City = client.City,
                ZipCode = client.ZipCode,
                Province = client.Province,
                Country = client.Country,
                ContactName = client.ContactName,
                ContactPhone = client.ContactPhone,
                ContactEmail = client.ContactEmail
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] ClientRequest client)
        {
            try
            {
                int newClientId = await _clientService.AddClient(client);
                return GetClientById(newClientId);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id != client.Id)
            {
                return BadRequest("Client ID in the path does not match the ID in the body.");
            }

            try
            {
                await _clientService.UpdateClient(client);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            _clientService.DeleteClient(id);
            return NoContent();
        }
    }
}
