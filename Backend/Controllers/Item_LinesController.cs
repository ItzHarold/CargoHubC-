using System.Collections.Generic;
using Backend.Features.ItemLines;
using Backend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.ItemLines
{
    [ApiController]
    [Route("api/item_lines")]
    public class ItemLinesController : ControllerBase
    {
        private IItemLineService _itemLineservice { get; set; }

        public ItemLinesController(IItemLineService itemLineservice)
        {
            _itemLineservice = itemLineservice;
        }

        [HttpGet]
        public IActionResult GetItemLines()
        {
            return Ok(_itemLineservice.GetAllItemLines());
        }

        [HttpGet("{id}")]
        public IActionResult GetItemLineById(int id)
        {
            var itemLine = _itemLineservice.GetItemLineById(id);
            if (itemLine == null)
            {
                return NotFound();
            }

            var response = new ItemLineResponse
            {
                id = itemLine.id,
                Name = itemLine.Name,
                Description = itemLine.Description
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemLine(ItemLineRequest itemLine)
        {
            try
            {
                int newItemLineId = await _itemLineservice.AddItemLine(itemLine);
                return GetItemLineById(newItemLineId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteItemLine(int id)
        {
            _itemLineservice.DeleteItemLine(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemGroup(int id, [FromBody] ItemLine itemLine)
        {
            if (id != itemLine.id)
            {
                return BadRequest("Item Group does not match the ID");
            }
            try
            {
                await _itemLineservice.UpdateItemLine(itemLine);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            
        }
    }
}
