using Backend.Features.Items;
using Backend.Request;
using Backend.Response;
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
        public IActionResult AddItem([FromBody] ItemRequest itemRequest)
        {
            _service.AddItem(itemRequest);
            return Ok();
        }

        [HttpGet("{uid}", Name = "GetItemById")]
        public IActionResult GetItemById(string uid)
        {
            var item = _service.GetItemById(uid);
            if (item == null)
            {
                return NotFound();
            }

            var response = new ItemResponse
            {
                Uid = item.Uid,
                Code = item.Code,
                Description = item.Description,
                ShortDescription = item.ShortDescription,
                UpcCode = item.UpcCode,
                ModelNumber = item.ModelNumber,
                CommodityCode = item.CommodityCode,
                ItemLineId = item.ItemLineId,
                ItemGroupId = item.ItemGroupId,
                ItemTypeId = item.ItemTypeId,
                UnitPurchaseQuantity = item.UnitPurchaseQuantity,
                UnitOrderQuantity = item.UnitOrderQuantity,
                PackOrderQuantity = item.PackOrderQuantity,
                SupplierId = item.SupplierId,
                SupplierCode = item.SupplierCode,
                SupplierPartNumber = item.SupplierPartNumber,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return Ok(response);
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
        public IActionResult UpdateItem(string uid, [FromBody] ItemRequest itemRequest)
        {
            _service.UpdateItem(uid, itemRequest);
            return NoContent();
        }

        [HttpGet("supplier/{supplierId}", Name = "GetItemsBySupplierId")]
        public IActionResult GetItemsBySupplierId(int supplierId)
        {
            var items = _service.GetItemsBySupplierId(supplierId);
            return Ok(items);
        }

        [HttpGet("group/{itemGroupId}", Name = "GetItemsByItemGroupId")]
        public IActionResult GetItemsByItemGroupId(int itemGroupId)
        {
            var items = _service.GetItemsByItemGroupId(itemGroupId);
            if (items == null || !items.Any())
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpGet("line/{itemLineId}", Name = "GetItemsByItemLineId")]
        public IActionResult GetItemsByItemLineId(int itemLineId)
        {
            var items = _service.GetItemsByItemLineId(itemLineId);
            if (items == null || !items.Any())
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpGet("type/{itemTypeId}", Name = "GetItemsByItemTypeId")]
        public IActionResult GetItemsByItemTypeId(int itemTypeId)
        {
            // Debugging - log the request
            Console.WriteLine($"Request received for ItemTypeId: {itemTypeId}");

            var items = _service.GetItemsByItemTypeId(itemTypeId);

            // Debugging - check if any items were returned
            if (items == null || !items.Any())
            {
                // Log the result
                Console.WriteLine($"No items found for ItemTypeId: {itemTypeId}");
                return NotFound();
            }

            return Ok(items);
        }

    }
}
