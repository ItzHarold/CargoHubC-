using System.Collections.Generic;
using Backend.Features.Locations;
using Backend.Request;
using Backend.Response;
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

            var response = new LocationResponse
            {
                Id = location.Id,
                WarehouseId = location.WarehouseId,
                Code = location.Code,
                Name = $"Row: {location.Row}, Rack: {location.Rack}, Shelf: {location.Shelf}",
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] LocationRequest locationRequest)
        {
            try
            {
                var createdLocation = await _locationService.AddLocation(locationRequest);
                var response = new LocationResponse
                {
                    Id = createdLocation.Id,
                    WarehouseId = createdLocation.WarehouseId,
                    Code = createdLocation.Code,
                    Name = $"Row: {createdLocation.Row}, Rack: {createdLocation.Rack}, Shelf: {createdLocation.Shelf}",
                };

                return CreatedAtAction(nameof(GetLocationById), new { id = response.Id }, response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] LocationRequest locationRequest)
        {
            try
            {
                await _locationService.UpdateLocation(id, locationRequest);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
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
