using Backend.Features.Items;
using Microsoft.AspNetCore.Mvc;
using Backend.Features.Orders;
using Backend.Request;

namespace Backend.Controllers.Orders
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequest orderRequest)
        {
            int newOrderId = await _orderService.AddOrder(orderRequest);
            var order = _orderService.GetOrderById(newOrderId);
            if (order == null)
                return NotFound();

            var response = _orderService.MapToResponse(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = response.Id }, response);
        }

        [HttpGet]
        public IEnumerable<Order> GetAllOrders()
        {
            return _orderService.GetAllOrders();
        }

        [HttpGet("{id}")]
        public Order? GetOrderById(int id)
        {
            return _orderService.GetOrderById(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderRequest orderRequest)
        {
            try
            {
                await _orderService.UpdateOrder(id, orderRequest);
                var updatedOrder = _orderService.GetOrderById(id);
                if (updatedOrder == null)
                    return NotFound();

                var response = _orderService.MapToResponse(updatedOrder);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public void DeleteOrder(int id)
        {
            _orderService.DeleteOrder(id);
        }

        [HttpPut("{orderId}/items/{itemUid}")]
        public IActionResult UpdateItemInOrder(int orderId, string itemUid, [FromBody] Item item)
        {
            try
            {
                _orderService.UpdateItemInOrder(orderId, itemUid, item);
                return NoContent(); // Successfully updated the item
            }
            catch (ArgumentException ex)
            {
                // If the item or order is not found, return a NotFound response with the message
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/items")]
        public IActionResult GetItemsForOrder(int id)
        {
            // Fetch the items for the given order ID
            var items = _orderService.GetItemsByOrderId(id);

            if (items.Any())
            {
                return Ok(items); // Return the list of items if found
            }
            return NotFound("No items found for this order."); // Return NotFound if no items exist
        }
    }
}
