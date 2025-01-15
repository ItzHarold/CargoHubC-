using System.Collections.Generic;
using Backend.Features.ItemLines;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.ItemLinesController
{
    [ApiController]
    [Route("api/item_lines")]
    public class ItemLinesController : ControllerBase
    {
        private readonly IItemLineService _itemLineService;

        public ItemLinesController(IItemLineService itemLineService)
        {
            _itemLineService = itemLineService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ItemLine>> GetAllItemLines(
            [FromQuery] Dictionary<string, string?>? filters = null, 
            [FromQuery] string? sortBy = null, 
            [FromQuery] bool sortDescending = false)
        {
            var itemLine = _itemLineService.GetAllItemLines(filters, sortBy, sortDescending);
            return Ok(itemLine);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetItemLineById(int id)
        {
            var line = _itemLineService.GetItemLineById(id);
            return line is not null ? Ok(line) : NotFound();
        }

        [HttpPost]
        public IActionResult AddItemLine([FromBody] ItemLine itemLine)
        {
            _itemLineService.AddItemLine(itemLine);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteItemLine(int id)
        {
            _itemLineService.DeleteItemLine(id);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateItemLine(int id, [FromBody] ItemLine itemLine)
        {
            _itemLineService.UpdateItemLine(id, itemLine);
            return NoContent();
        }
    }
}
