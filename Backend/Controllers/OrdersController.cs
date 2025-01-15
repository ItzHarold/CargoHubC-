using System.Collections.Generic;
using Backend.Features.Orders;
using Microsoft.AspNetCore.Mvc;

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
        public void AddOrder([FromBody] Order order)
        {
            _orderService.AddOrder(order);
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

        [HttpPut]
        public void UpdateOrder([FromBody] Order order)
        {
            _orderService.UpdateOrder(order);
        }

        [HttpDelete("{id}")]
        public void DeleteOrder(int id)
        {
            _orderService.DeleteOrder(id);
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
