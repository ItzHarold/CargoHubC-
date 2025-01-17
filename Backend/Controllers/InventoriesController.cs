using System.Collections.Generic;
using Backend.Features.Inventories;
using Backend.Response;
using FluentValidation;
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

            var response = new InventoryResponse
            {
                Id = inventory.Id,
                ItemId = inventory.ItemId,
                Description = inventory.Description,
                ItemReference = inventory.ItemReference,
                LocationId = inventory.LocationId,
                TotalOnHand = inventory.TotalOnHand,
                TotalExpected = inventory.TotalExpected,
                TotalOrdered = inventory.TotalOrdered,
                TotalAllocated = inventory.TotalAllocated,
                TotalAvailable = inventory.TotalAvailable
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory([FromBody] InventoryRequest inventoryRequest)
        {
            try
            {
                int newInventoryId = await _inventoryService.AddInventory(inventoryRequest);
                return GetInventoryById(newInventoryId);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
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
