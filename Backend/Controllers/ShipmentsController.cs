using System.Collections.Generic;
using Backend.Features.Shipments;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Backend.Requests;
using Backend.Response;
using Backend.Features.Orders;

namespace Backend.Controllers.Shipments
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }


        [HttpGet(Name = "GetAllShipments")]
        public IActionResult GetAllShipments(
            string? sort,
            string? direction,
            int? sourceId,
            DateTime? orderDate,
            DateTime? requestDate,
            DateTime? shipmentDate,
            string? shipmentType,
            string? shipmentStatus,
            string? carrierCode,
            string? paymentType,
            string? transferMode,
            int? totalPackageCount,
            float? totalPackageWeight)
        {
            var shipments = _shipmentService.GetAllShipments(
                sort,
                direction,
                sourceId,
                orderDate,
                requestDate,
                shipmentDate,
                shipmentType,
                shipmentStatus,
                carrierCode,
                paymentType,
                transferMode,
                totalPackageCount,
                totalPackageWeight
            );

            return Ok(shipments);
        }


        [HttpGet("{id}")]
        public Shipment? GetShipmentById(int id)
        {
            return _shipmentService.GetShipmentById(id);
        }


        [HttpPost]
        public async Task<IActionResult> AddShipment([FromBody] ShipmentRequest shipmentRequest)
        {
            int newShipmentId = await _shipmentService.AddShipment(shipmentRequest);
            var shipment = _shipmentService.GetShipmentById(newShipmentId);
            if (shipment == null)
                return NotFound();
            
            var response = _shipmentService.MapToResponse(shipment);
            return CreatedAtAction(nameof(GetShipmentById), new { id = response.Id}, response);
           
        }

        // ShipmentsController.cs
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] ShipmentRequest request)
        {
            try
            {
                await _shipmentService.UpdateShipment(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public void DeleteShipment(int id)
        {
            _shipmentService.DeleteShipment(id);
        }

    }

}
