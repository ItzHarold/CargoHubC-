using System.Collections.Generic;
using Backend.Features.Suppliers;
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
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
        {
            try
            {
                await _supplierService.AddSupplier(supplier);
                return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.Id }, supplier);
            }
            catch (ValidationException ex)
            {
                return BadRequest(FormatValidationErrors(ex.Errors));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest(new { message = "Supplier ID in the path does not match the ID in the body." });
            }

            try
            {
                await _supplierService.UpdateSupplier(supplier);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(FormatValidationErrors(ex.Errors));
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
