using System.Collections.Generic;
using Backend.Features.Suppliers;
using Backend.Response;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Suppliers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult GetAllSuppliers()
        {
            var suppliers = _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public IActionResult GetSupplierById(int id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Supplier not found." });
            }

            // Convert Supplier entity to SupplierResponse
            var response = new SupplierResponse
            {
                Id = supplier.Id,
                Code = supplier.Code,
                Name = supplier.Name,
                Address = supplier.Address,
                AddressExtra = supplier.AddressExtra,
                City = supplier.City,
                ZipCode = supplier.ZipCode,
                Province = supplier.Province,
                Country = supplier.Country,
                ContactName = supplier.ContactName,
                PhoneNumber = supplier.PhoneNumber,
                Reference = supplier.Reference,
                // Convert Items to ItemResponse
                Items = supplier.Items?.Select(item => new ItemResponse
                {
                    Uid = item.Uid,
                    Code = item.Code,
                    Description = item.Description,
                    ShortDescription = item.ShortDescription,
                    UpcCode = item.UpcCode,
                    ModelNumber = item.ModelNumber,
                    CommodityCode = item.CommodityCode,
                    SupplierId = item.SupplierId,
                    SupplierPartNumber = item.SupplierPartNumber
                }).ToList() ?? new List<ItemResponse>() // Ensure an empty list if no items are present
            };

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierRequest supplierRequest)
        {
            try
            {
                var createdSupplier = await _supplierService.AddSupplier(supplierRequest);
                return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierRequest supplierRequest)
        {
            try
            {
                await _supplierService.UpdateSupplier(id, supplierRequest);
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
        public IActionResult DeleteSupplier(int id)
        {
            _supplierService.DeleteSupplier(id);
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
