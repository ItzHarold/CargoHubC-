using System.Collections.Generic;
using Backend.Features.Transfers;
using Backend.Features.Items;
using Microsoft.AspNetCore.Mvc;
using Backend.Response;
using FluentValidation;

namespace Backend.Controllers.Transfers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpGet(Name = "GetAllTransfers")]
        public IActionResult GetAllTransfers(
            string? sort,
            string? direction,
            string? reference,
            string? transferStatus,
            int? transferToLocationId,
            int? transferFromLocationId)
        {
            var transfers = _transferService.GetAllTransfers(
                sort,
                direction,
                reference,
                transferStatus,
                transferToLocationId,
                transferFromLocationId
            );

            return Ok(transfers);
        }


        [HttpPost]
        public IActionResult AddTransfer([FromBody] TransferRequest transferRequest)
        {
            if (transferRequest.Items == null || transferRequest.Items.Count == 0)
            {
                return BadRequest("Transfer must include at least one item");
            }
            try
            {
                // Call the service method to add the transfer
                _transferService.AddTransfer(transferRequest);

                return CreatedAtAction(nameof(GetTransferById), new { id = transferRequest.Reference }, transferRequest);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTransferById(int id)
        {
            var transfer = _transferService.GetTransferById(id);
            if (transfer == null)
            {
                return NotFound();
            }

            // Convert Transfer entity to TransferResponse
            var response = new TransferResponse
            {
                Id = transfer.Id,
                Reference = transfer.Reference,
                TransferFrom = transfer.TransferFrom?.Id,
                TransferTo = transfer.TransferTo?.Id,
                TransferStatus = transfer.TransferStatus,
                // Convert TransferItems to TransferItemResponse
                Items = transfer.TransferItems?.Select(item => new TransferItemResponse
                {
                    ItemUid = item.ItemUid,
                    Amount = item.Amount
                }).ToList()
            };

            return Ok(response);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransfer(int id, [FromBody] TransferRequest request)
        {
            try
            {
                // Update the transfer using the service method
                await _transferService.UpdateTransfer(id, request);

                // If update is successful, return a NoContent response
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // If the transfer was not found, return a NotFound response with the error message
                return NotFound(new { error = ex.Message });
            }
            catch (ValidationException ex)
            {
                // If validation fails, return a BadRequest response with validation errors
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                // Catch all other exceptions and return a 500 Internal Server Error with the error message
                return StatusCode(500, new { error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteTransfer(int id)
        {
            var transfer = _transferService.GetTransferById(id);
            if (transfer == null)
            {
                return NotFound();
            }

            _transferService.DeleteTransfer(id);
            return NoContent();
        }

        [HttpGet("{id}/items")]
        public IActionResult GetItemsInTransfer(int id)
        {
            var transfer = _transferService.GetTransferById(id);
            if (transfer == null)
            {
                return NotFound("Transfer not found");
            }

            var items = transfer.TransferItems?.Select(ti => new
            {
                ItemUid = ti.ItemUid,
                Amount = ti.Amount
            });

            return Ok(items);
        }

        [HttpPost("{id}/commit")]
        public IActionResult CommitTransfer(int id)
        {
            try
            {
                // Call the service method to commit the transfer
                _transferService.CommitTransfer(id);

                return NoContent();  // Return 204 No Content for successful commit
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
