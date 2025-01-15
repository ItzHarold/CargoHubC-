using System.Collections.Generic;
using Backend.Features.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Locations
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public IActionResult GetAllLocations()
        {
            var locations = _locationService.GetAllLocations();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public IActionResult GetLocationById(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound(new { message = "Location not found." });
            }
            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] IncomingLocation incomingLocation)
        {
            try
            {
                await _locationService.AddLocation(incomingLocation);
                return CreatedAtAction(nameof(GetLocationById), new { id = incomingLocation.Id }, incomingLocation);
            }
            catch (ValidationException ex)
            {
                return BadRequest(FormatValidationErrors(ex.Errors));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] IncomingLocation incomingLocation)
        {
            if (id != incomingLocation.Id)
            {
                return BadRequest(new { message = "Location ID in the path does not match the ID in the body." });
            }

            try
            {
                await _locationService.UpdateLocation(incomingLocation);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(FormatValidationErrors(ex.Errors));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLocation(int id)
        {
            _locationService.DeleteLocation(id);
            return NoContent();
        }

        private static object FormatValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            return new
            {
                errors = errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            };
        }
    }
}
