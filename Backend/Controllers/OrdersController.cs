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

        [HttpGet("{id}/customer")]
        public void GetOrdersFromCustomers(int id)
        {
            _orderService.GetOrdersFromCustomers(id);
        }

    }
}
