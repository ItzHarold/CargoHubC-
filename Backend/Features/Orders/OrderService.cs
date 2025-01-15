using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Backend.Features.Items;

namespace Backend.Features.Orders
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        IEnumerable<Item> GetItemsByOrderId(int orderId);
    }

    public class OrderService: IOrderService
    {
        private readonly CargoHubDbContext _dbContext;
        public OrderService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddOrder(Order order)
        {
            order.CreatedAt = DateTime.Now;
            _dbContext.Orders?.Add(order);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            if (_dbContext.Orders != null)
            {
                return _dbContext.Orders.ToList();
            }
            return new List<Order>();
        }
        public Order? GetOrderById(int id)
        {
            return _dbContext.Orders?.FirstOrDefault(o => o.Id == id);
        }
        public void UpdateOrder(Order order)
        {
            order.UpdatedAt = DateTime.Now;
            _dbContext.Orders?.Update(order);
            _dbContext.SaveChanges();
        }
        public void DeleteOrder(int id)
        {
            var order = _dbContext.Orders
                ?.Include(o => o.Items)
                .FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                if (order.Items != null)
                {
                    _dbContext.Items?.RemoveRange(order.Items);
                }

                _dbContext.Orders?.Remove(order);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Item> GetItemsByOrderId(int orderId)
        {
            // Ensure that the Orders DbSet is not null
            if (_dbContext.Orders == null)
            {
                return Enumerable.Empty<Item>(); // Return an empty collection if Orders is null
            }

            // Retrieve the order and include its Items collection
            var order = _dbContext.Orders
                .Include(o => o.Items) // Ensure that the Items are included in the query
                .FirstOrDefault(o => o.Id == orderId); // Get the first order matching the orderId or null

            if (order == null)
            {
                // Log or debug this if order is not found
                Console.WriteLine($"Order with ID {orderId} not found.");
                return Enumerable.Empty<Item>(); // Return an empty collection if the order is not found
            }

            if (order.Items == null || order.Items.Count == 0)
            {
                // Log or debug this if the order has no items
                Console.WriteLine($"Order with ID {orderId} has no items.");
                return Enumerable.Empty<Item>(); // Return an empty collection if the order has no items
            }

            // Return the list of items for the order
            return order.Items; 
        }
    }
}
