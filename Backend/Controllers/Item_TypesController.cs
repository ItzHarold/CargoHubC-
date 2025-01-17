using System.Collections.Generic;
using Backend.Features.ItemTypes;
using Backend.Requests;
using Backend.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.ItemTypes
{
    [ApiController]
    [Route("api/item_types")]
    public class ItemTypesController : ControllerBase
    {
        private IItemTypeService _itemTypeService;

        public ItemTypesController(IItemTypeService itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }

        [HttpGet]
        public IActionResult GetAllItemTypes()
        {
            return Ok(_itemTypeService.GetAllItemTypes());
        }

        [HttpGet("{id}")]
        public IActionResult GetItemTypeById(int id)
        {
            var itemGroup = _itemTypeService.GetItemTypeById(id);
            if (itemGroup == null)
            {
                return NotFound();
            }

            var response = new ItemTypeResponse
            {
                Id = itemGroup.Id,
                Name = itemGroup.Name,
                Description = itemGroup.Description
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemType(ItemTypeRequest itemType)
        {
            try
            {
                int newItemTypeId = await _itemTypeService.AddItemType(itemType);
                return GetItemTypeById(newItemTypeId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteItemType(int id)
        {
            _itemTypeService.DeleteItemType(id);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemType(int id, [FromBody] ItemType itemType)
        {
            if (id != itemType.Id)
            {
                return BadRequest("Item Group does not match the ID");
            }
            try
            {
                await _itemTypeService.UpdateItemType(itemType);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            
        }
    }
}
