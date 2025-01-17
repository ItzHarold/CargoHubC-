using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Features.ItemGroups;
using Backend.Response;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Backend.Request;

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

            var response = new ItemGroupResponse
            {
                Id = itemGroup.Id,
                Name = itemGroup.Name,
                Description = itemGroup.Description
            };

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> AddItemGroup(ItemGroupRequest itemGroup)
        {
            try
            {
                int newItemGroupId = await _itemGroupService.AddItemGroup(itemGroup);
                return GetItemGroupById(newItemGroupId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemGroup(int id, [FromBody] ItemGroup itemGroup)
        {
            if (id != itemGroup.Id)
            {
                return BadRequest("Item Group does not match the ID");
            }
            try
            {
                await _itemGroupService.UpdateItemGroup(itemGroup);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItemGroup(int id)
        {
            _itemGroupService.DeleteItemGroup(id);
            return NoContent();
        }
    }
}
