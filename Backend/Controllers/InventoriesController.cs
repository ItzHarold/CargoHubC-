using System.Collections.Generic;
using Backend.Features.Inventories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Inventories
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Inventory>> GetAllContacts(
            [FromQuery] Dictionary<string, string?>? filters = null, 
            [FromQuery] string? sortBy = null, 
            [FromQuery] bool sortDescending = false)
        {
            var inventory = _inventoryService.GetAllContacts(filters, sortBy, sortDescending);
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public IActionResult GetInventoryById(int id)
        {
            var inventory = _inventoryService.GetInventoryById(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return Ok(inventory);
        }

        public IActionResult AddInventory(Inventory inventory)
        {
            _inventoryService.AddInventory(inventory);
            return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInventory(int id, Inventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest();
            }
            _inventoryService.UpdateInventory(inventory);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInventory(int id)
        {
            _inventoryService.DeleteInventory(id);
            return NoContent();
        }
    }
}
