using System.Collections.Generic;
using Backend.Features.Transfers;
using Backend.Features.Items;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IEnumerable<Transfer> GetAllTransfers()
        {
            return _transferService.GetAllTransfers();
        }

        [HttpPost]
        public IActionResult AddTransfer([FromBody] Transfer transfer)
        {
            if (transfer.TransferItems == null || transfer.TransferItems.Count == 0)
            {
                return BadRequest("Transfer must include at least one item");
            }

            try
            {
                _transferService.AddTransfer(transfer);
                return CreatedAtAction(nameof(GetTransferById), new { id = transfer.Id }, transfer);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public ActionResult<Transfer> GetTransferById(int id)
        {
            var transfer = _transferService.GetTransferById(id);
            if (transfer == null)
            {
                return NotFound();
            }
            return transfer;
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTransfer(int id, [FromBody] Transfer transfer)
        {
            if (id != transfer.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTransfer = _transferService.GetTransferById(id);
            if (existingTransfer == null)
            {
                return NotFound();
            }

            try
            {
                _transferService.UpdateTransfer(transfer);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
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

            var items = transfer.TransferItems.Select(ti => new
            {
                ItemUid = ti.ItemUid,
                Amount = ti.Amount
            });

            return Ok(items);
        }
    }
}