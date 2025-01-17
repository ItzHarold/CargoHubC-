using Backend.Features.Items;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Items
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _service;

        public ItemsController(IItemService service)
        {
            _service = service;
        }

        [HttpPost(Name = "AddItem")]
        public IActionResult AddItem([FromBody] Item item)
        {
            _service.AddItem(item);
            return Ok();
        }

        [HttpGet("{uid}", Name = "GetItemById")]
        public IActionResult GetItemById(string uid)
        {
            var item = _service.GetItemById(uid);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpGet(Name = "GetAllItems")]
        public IActionResult GetAllItems()
        {
            return Ok(_service.GetAllItems());
        }

        [HttpDelete("{uid}", Name = "DeleteItem")]
        public IActionResult DeleteItem(string uid)
        {
            _service.DeleteItem(uid);
            return NoContent();
        }

        [HttpPut("{uid}", Name = "UpdateItem")]
        public IActionResult UpdateItem(string uid, [FromBody] Item item)
        {
            _service.UpdateItem(uid, item);
            return NoContent();
        }

        [HttpGet("supplier/{supplierId}", Name = "GetItemsBySupplierId")]
        public IActionResult GetItemsBySupplierId(int supplierId)
        {
            var items = _service.GetItemsBySupplierId(supplierId);
            return Ok(items);
        }

        [HttpGet("by-item-type/{itemTypeId:int}")]
        public IActionResult GetItemsByItemType(int itemTypeId)
        {
            var items = _service.GetItemsByItemType(itemTypeId);
            return Ok(items);
        }

        [HttpGet("by-item-group/{itemGroupId:int}")]
        public IActionResult GetItemsByItemGroup(int itemGroupId)
        {
            var items = _service.GetItemsByItemGroup(itemGroupId);
            return Ok(items);
        }

        [HttpGet("by-item-line/{itemLineId:int}")]
        public IActionResult GetItemsByItemLine(int itemLineId)
        {
            var items = _service.GetItemsByItemLine(itemLineId);
            return Ok(items);
        }
    }
}
