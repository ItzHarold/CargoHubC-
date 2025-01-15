using Backend.Infrastructure.Database;
using Backend.Features.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

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
        void UpdateItemInOrder(int orderId, string itemUid, Item updatedItem);
    }

    public class OrderService: IOrderService
    {
        private readonly CargoHubDbContext _dbContext;

        public OrderService(CargoHubDbContext dbContext)
        {
            // Ensure that the DbContext is not null when the service is constructed
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Throws if dbContext is null
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
            if (_dbContext.Orders == null)
            {
                return Enumerable.Empty<Item>(); // Return an empty collection if Orders is null
            }

            var order = _dbContext.Orders
                .Include(o => o.Items) // Ensure that the Items are included in the query
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                Console.WriteLine($"Order with ID {orderId} not found.");
                return Enumerable.Empty<Item>();
            }

            if (order.Items == null || order.Items.Count == 0)
            {
                Console.WriteLine($"Order with ID {orderId} has no items.");
                return Enumerable.Empty<Item>();
            }

            return order.Items;
        }

        public void UpdateItemInOrder(int orderId, string itemUid, Item updatedItem)
        {
            if (_dbContext.Orders == null)
            {
                throw new InvalidOperationException("Orders DbSet is null");
            }

            if (_dbContext.Items == null)
            {
                throw new InvalidOperationException("Items DbSet is null");
            }

            var order = _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found.");
            }

            if (order.Items == null)
            {
                throw new InvalidOperationException($"Items collection is null for order {orderId}");
            }

            var item = order.Items.FirstOrDefault(i => i.Uid == itemUid);

            if (item == null)
            {
                throw new ArgumentException($"Item with UID {itemUid} not found in order {orderId}.");
            }

            // Apply updates to the item
            item.Code = updatedItem.Code;
            item.Description = updatedItem.Description;
            item.ShortDescription = updatedItem.ShortDescription;
            item.UpcCode = updatedItem.UpcCode;
            item.ModelNumber = updatedItem.ModelNumber;
            item.CommodityCode = updatedItem.CommodityCode;
            item.UnitPurchaseQuantity = updatedItem.UnitPurchaseQuantity;
            item.UnitOrderQuantity = updatedItem.UnitOrderQuantity;
            item.PackOrderQuantity = updatedItem.PackOrderQuantity;
            item.SupplierCode = updatedItem.SupplierCode;
            item.SupplierPartNumber = updatedItem.SupplierPartNumber;

            // Update the item in the database
            _dbContext.Items.Update(item);
            _dbContext.SaveChanges();
        }

    }
}
