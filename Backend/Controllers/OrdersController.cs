using Backend.Features.Items;
using Microsoft.AspNetCore.Mvc;
using Backend.Features.Orders;
using Backend.Request;
using Backend.Features.OrderItems;

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

        [HttpGet(Name = "GetAllOrders")]
        public IActionResult GetAllOrders(
            string? sort,
            string? direction,
            string? reference,
            float? totalAmount,
            float? totalDiscount,
            float? totalTax,
            float? totalSurcharge,
            string? orderStatus,
            int? warehouseId)
        {
            var orders = _orderService.GetAllOrders(
                sort,
                direction,
                reference,
                totalAmount,
                totalDiscount,
                totalTax,
                totalSurcharge,
                orderStatus,
                warehouseId
            );

            return Ok(orders);
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
        public IActionResult UpdateItemInOrder(int orderId, string itemUid, [FromBody] OrderItemUpdateRequest updateRequest)
        {
            if (updateRequest == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                if (string.IsNullOrEmpty(updateRequest.ItemUid) || updateRequest.Amount <= 0)
                {
                    return BadRequest("ItemUid and Amount are required.");
                }

                // Call the service to handle the logic of updating the order item
                _orderService.UpdateOrderItem(orderId, itemUid, updateRequest);

                return NoContent(); // Indicate successful update with no content to return
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the item.", details = ex.Message });
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

        [HttpGet("client/{clientId}/orders")]
        public IActionResult GetOrdersByClientId(int clientId)
        {
            var orders = _orderService.GetOrdersByClientId(clientId);
            
            if (!orders.Any())
            {
                return NotFound(new { message = "No orders found for this client." });
            }

            var orderResponses = orders.Select(o => _orderService.MapToResponse(o)).ToList();
            return Ok(orderResponses);
        }

    }
}
