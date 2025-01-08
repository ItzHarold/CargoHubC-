using System.Collections.Generic;
using Backend.Features.Warehouses;
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
        public IEnumerable<Warehouse> GetAllWarehouses()
        {
            return _warehouseService.GetAllWarehouses();
        }

        [HttpGet("{id}")]
        public Warehouse? GetWarehouseById(int id)
        {
            return _warehouseService.GetWarehouseById(id);
        }

        [HttpPost]
        public void AddWarehouse([FromBody] Warehouse warehouse)
        {
            _warehouseService.AddWarehouse(warehouse);
        }

        [HttpPut]
        public void UpdateWarehouse([FromBody] Warehouse warehouse)
        {
            _warehouseService.UpdateWarehouse(warehouse);
        }

        [HttpDelete("{id}")]
        public void DeleteWarehouse(int id)
        {
            _warehouseService.DeleteWarehouse(id);
        }
    }
}
