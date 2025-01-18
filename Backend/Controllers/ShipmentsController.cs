using System.Collections.Generic;
using Backend.Features.Shipments;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Backend.Requests;
using Backend.Response;

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

        [HttpGet]
        public IEnumerable<Shipment> GetAllShipments()
        {
            return _shipmentService.GetAllShipments();
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
            {
                return NotFound();
            }

            var response = new ShipmentResponse
            {
                SourceId = shipment.SourceId,
                OrderDate = shipment.OrderDate,
                RequestDate = shipment.RequestDate,
                ShipmentDate = shipment.ShipmentDate,
                ShipmentType = shipment.ShipmentType,
                ShipmentStatus = shipment.ShipmentStatus,
                Notes = shipment.Notes,
                CarrierCode = shipment.CarrierCode,
                CarrierDescription = shipment.CarrierDescription,
                ServiceCode = shipment.ServiceCode,
                PaymentType = shipment.PaymentType,
                TransferMode = shipment.TransferMode,
                TotalPackageCount = shipment.TotalPackageCount,
                TotalPackageWeight = shipment.TotalPackageWeight
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddShipment([FromBody] ShipmentRequest shipment)
        {
            try
            {
                int newShipmentId = await _shipmentService.AddShipment(shipment);
                return GetClientById(newShipmentId);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
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
