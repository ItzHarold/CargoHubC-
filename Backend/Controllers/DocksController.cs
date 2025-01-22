using Backend.Features.Docks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Docks
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocksController : ControllerBase
    {
        private readonly IDockService _service;

        public DocksController(IDockService service)
        {
            _service = service;
        }

        [HttpPost(Name = "CreateDock")]
        public IActionResult CreateDock([FromBody] Dock dock)
        {
            try
            {
                _service.CreateDock(dock);
                return Ok(new { message = "Dock created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}", Name = "UpdateDock")]
        public IActionResult UpdateDock(int id, [FromBody] Dock dock)
        {
            var updated = _service.UpdateDock(id, dock);
            return updated ? Ok(new { message = "Dock updated successfully." }) : BadRequest(new { error = "Cannot update ShipmentId unless the dock is unoccupied." });
        }

        [HttpPut("{id}/clear", Name = "ClearDock")]
        public IActionResult ClearDock(int id)
        {
            var cleared = _service.ClearDock(id);
            return cleared ? Ok(new { message = "Dock cleared successfully." }) : NotFound(new { error = "Dock not found." });
        }

        [HttpGet(Name = "GetAllDocks")]
        public IActionResult GetAllDocks()
        {
            var docks = _service.GetAllDocks();
            return Ok(docks);
        }

        [HttpGet("{id}", Name = "GetDockById")]
        public IActionResult GetDockById(int id)
        {
            var dock = _service.GetDockById(id);
            return dock != null ? Ok(dock) : NotFound(new { error = "Dock not found." });
        }

        [HttpDelete("{id}", Name = "DeleteDock")]
        public IActionResult DeleteDock(int id)
        {
            var deleted = _service.DeleteDock(id);
            return deleted ? Ok(new { message = "Dock deleted successfully." }) : NotFound(new { error = "Dock not found." });
        }
    }
}
