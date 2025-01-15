using System.Collections.Generic;
using Backend.Features.Warehouses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Warehouses
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public IActionResult GetAllWarehouses()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public IActionResult GetWarehouseById(int id)
        {
            var warehouse = _warehouseService.GetWarehouseById(id);
            if (warehouse == null)
            {
                return NotFound(new { message = "Warehouse not found." });
            }
            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] Warehouse warehouse)
        {
            try
            {
                await _warehouseService.AddWarehouse(warehouse);
                return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id }, warehouse);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return BadRequest(new { message = "Warehouse ID in the path does not match the ID in the body." });
            }

            try
            {
                await _warehouseService.UpdateWarehouse(warehouse);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            _warehouseService.DeleteWarehouse(id);
            return NoContent();
        }
    }
}
