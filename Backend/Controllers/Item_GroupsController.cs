using System.Collections.Generic;
using Backend.Features.ItemGroups;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.ItemGroupsController
{
    [ApiController]
    [Route("api/item_groups")]
    public class ItemGroupsController : ControllerBase
    {
        private readonly IItemGroupService _itemGroupService;

        public ItemGroupsController(IItemGroupService itemGroupService)
        {
            _itemGroupService = itemGroupService;
        }

        [HttpGet]
        public IActionResult GetAllItemGroups()
        {
            var itemGroups = _itemGroupService.GetAllItemGroups();
            return Ok(itemGroups);
        }

        [HttpGet("{id}")]
        public IActionResult GetItemGroupById(int id)
        {
            var itemGroup = _itemGroupService.GetItemGroupById(id);
            if (itemGroup == null)
            {
                return NotFound();
            }
            return Ok(itemGroup);
        }

        public IActionResult AddItemGroup(ItemGroup itemGroup)
        {
            _itemGroupService.AddItemGroup(itemGroup);
            return CreatedAtAction(nameof(GetItemGroupById), new { id = itemGroup.Id }, itemGroup);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItemGroup(int id, ItemGroup itemGroup)
        {
            if (id != itemGroup.Id)
            {
                return BadRequest();
            }
            _itemGroupService.UpdateItemGroup(itemGroup);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemGroup(int id)
        {
            _itemGroupService.DeleteItemGroup(id);
            return NoContent();
        }
    }
}
