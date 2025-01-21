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
        public IActionResult GetAllInventories(
            string? sort,
            string? direction,
            string? itemId,
            int? totalOnHand,
            int? totalExpected,
            int? totalOrdered,
            int? totalAllocated,
            int? totalAvailable)
        {
            var inventories = _inventoryService.GetAllInventories(
                sort,
                direction,
                itemId,
                totalOnHand,
                totalExpected,
                totalOrdered,
                totalAllocated,
                totalAvailable);

            return Ok(inventories);
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
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryRequest inventoryRequest)
        {
            try
            {
                await _inventoryService.UpdateInventory(id, inventoryRequest);
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
        public IActionResult DeleteInventory(int id)
        {
            _inventoryService.DeleteInventory(id);
            return NoContent();
        }

        [HttpGet("with-locations")]
        public IActionResult GetInventoriesWithLocations([FromQuery] string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return BadRequest(new { message = "itemId is required." });
            }

            var inventoriesWithLocations = _inventoryService.GetInventoryWithLocations(itemId);

            if (!inventoriesWithLocations.Any())
            {
                return NotFound(new { message = $"No inventories found for itemId: {itemId}" });
            }

            return Ok(inventoriesWithLocations);
        }

        [HttpGet("total")]
        public IActionResult GetTotalInventoryByItemId([FromQuery] string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return BadRequest(new { message = "itemId is required." });
            }

            var totalInventory = _inventoryService.GetTotalInventoryByItemId(itemId);

            return Ok(new { itemId, totalInventory });
        }


    }
}
