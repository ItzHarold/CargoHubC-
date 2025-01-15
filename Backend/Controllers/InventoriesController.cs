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
        public IActionResult GetAllInventories()
        {
            var inventory = _inventoryService.GetAllInventories();
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

        [HttpPost]
        public async Task<IActionResult> AddInventory([FromBody] Inventory inventory)
        {
            await _inventoryService.AddInventory(inventory);
            return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] Inventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest("Inventory ID in the path does not match the ID in the body.");
            }

            await _inventoryService.UpdateInventory(inventory);
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
