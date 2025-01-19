using Backend.Infrastructure.Database;
using Backend.Features.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using Backend.Features.OrderItems;
using Backend.Request;
using Backend.Response;

namespace Backend.Features.Orders
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        Task<int> AddOrder(OrderRequest orderRequest);
        Task UpdateOrder(int orderId, OrderRequest orderRequest);
        void DeleteOrder(int id);
        IEnumerable<Item> GetItemsByOrderId(int orderId);
        void UpdateItemInOrder(int orderId, string itemUid, Item updatedItem);
        OrderResponse MapToResponse(Order order);
    }

    public class OrderService: IOrderService
    {
        private readonly CargoHubDbContext _dbContext;

        public OrderService(CargoHubDbContext dbContext)
        {
            // Ensure that the DbContext is not null when the service is constructed
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Throws if dbContext is null
        }

        public async Task<int> AddOrder(OrderRequest orderRequest)
        {

            if (_dbContext?.Warehouses?.Any(warehouse => warehouse.Id == orderRequest.WarehouseId) == false)
            {
                throw new ArgumentException("Warehouse not found");
            }

            if (_dbContext?.Contacts?.Any(contact => contact.Id == orderRequest.SourceId) == false)
            {
                throw new ArgumentException("Contact not found sourceId");
            }


            if (orderRequest.ShipTo.HasValue &&  _dbContext?.Clients?.Any(client => client.Id == orderRequest.ShipTo) == false)
            {
                throw new ArgumentException("Client not found for ShipTo");
            }

            if (orderRequest.BillTo.HasValue &&_dbContext?.Clients?.Any(client => client.Id == orderRequest.BillTo) == false)
            {
                throw new ArgumentException("Client not found for BillTo");
            }


            // Create the order object
            Order order = new()
            {
                RequestDate = orderRequest.RequestDate,
                Reference = orderRequest.Reference,
                ReferenceExtra = orderRequest.ReferenceExtra,
                Notes = orderRequest.Notes,
                ShippingNotes = orderRequest.ShippingNotes,
                PickingNotes = orderRequest.PickingNotes,
                TotalAmount = orderRequest.TotalAmount,
                TotalDiscount = orderRequest.TotalDiscount,
                TotalTax = orderRequest.TotalTax,
                TotalSurcharge = orderRequest.TotalSurcharge,
                OrderStatus = orderRequest.OrderStatus,
                OrderDate = orderRequest.OrderDate,
                WarehouseId = orderRequest.WarehouseId,
                SourceId = orderRequest.SourceId,
                BillToClientId = orderRequest.BillTo,
                ShipToClientId = orderRequest.ShipTo,
            };

            // Add the order to the database
            order.CreatedAt = DateTime.UtcNow;
            _dbContext?.Orders?.Add(order);

            if (_dbContext != null)
            {
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            return order.Id;
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

        public async Task UpdateOrder(int orderId, OrderRequest orderRequest)
        {
            if (_dbContext.Orders != null)
            {
                var order = _dbContext.Orders
                    .Include(o => o.OrderItems) // Ensure items are included
                    .FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                {
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                // Update basic properties
                order.SourceId = orderRequest.SourceId;
                order.OrderDate = orderRequest.OrderDate;
                order.RequestDate = orderRequest.RequestDate;
                order.Reference = orderRequest.Reference;
                order.ReferenceExtra = orderRequest.ReferenceExtra;
                order.OrderStatus = orderRequest.OrderStatus;
                order.Notes = orderRequest.Notes;
                order.ShippingNotes = orderRequest.ShippingNotes;
                order.PickingNotes = orderRequest.PickingNotes;
                order.WarehouseId = orderRequest.WarehouseId;
                order.TotalAmount = orderRequest.TotalAmount;
                order.TotalDiscount = orderRequest.TotalDiscount;
                order.TotalTax = orderRequest.TotalTax;
                order.TotalSurcharge = orderRequest.TotalSurcharge;

                // Update OrderItems if provided
                if (orderRequest.Items != null && orderRequest.Items.Count != 0)
                {
                    order.OrderItems = orderRequest.Items.Select(i => new OrderItem
                    {
                        ItemUid = i.ItemUid,
                        Amount = i.Amount,
                        OrderId = orderId
                    }).ToList();
                }

                order.UpdatedAt = DateTime.Now;
                _dbContext.Orders.Update(order);
            }

            await _dbContext.SaveChangesAsync();
        }

        public void DeleteOrder(int id)
        {
            var order = _dbContext.Orders
                ?.Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                if (order.OrderItems != null)
                {
                    // _dbContext.Items?.RemoveRange(order.OrderItems);
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
                .Include(o => o.OrderItems) // Ensure that the Items are included in the query
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                Console.WriteLine($"Order with ID {orderId} not found.");
                return Enumerable.Empty<Item>();
            }

            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                Console.WriteLine($"Order with ID {orderId} has no items.");
                return Enumerable.Empty<Item>();
            }

            return new List<Item>();
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
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found.");
            }

            if (order.OrderItems == null)
            {
                throw new InvalidOperationException($"Items collection is null for order {orderId}");
            }

            var item = order.OrderItems.FirstOrDefault(i => i.Item.Uid == itemUid);

            if (item == null)
            {
                throw new ArgumentException($"Item with UID {itemUid} not found in order {orderId}.");
            }

            // Apply updates to the item
            // item.Code = updatedItem.Code;
            // item.Description = updatedItem.Description;
            // item.ShortDescription = updatedItem.ShortDescription;
            // item.UpcCode = updatedItem.UpcCode;
            // item.ModelNumber = updatedItem.ModelNumber;
            // item.CommodityCode = updatedItem.CommodityCode;
            // item.UnitPurchaseQuantity = updatedItem.UnitPurchaseQuantity;
            // item.UnitOrderQuantity = updatedItem.UnitOrderQuantity;
            // item.PackOrderQuantity = updatedItem.PackOrderQuantity;
            // item.SupplierCode = updatedItem.SupplierCode;
            // item.SupplierPartNumber = updatedItem.SupplierPartNumber;
            //
            // // Update the item in the database
            // _dbContext.Items.Update(item);
            _dbContext.SaveChanges();
        }

        public OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                // SourceId = order.SourceId,
                // OrderDate = order.OrderDate,
                RequestDate = order.RequestDate,
                Reference = order.Reference,
                ReferenceExtra = order.ReferenceExtra,
                OrderStatus = order.OrderStatus,
                Notes = order.Notes,
                ShippingNotes = order.ShippingNotes,
                PickingNotes = order.PickingNotes,
                // WarehouseId = order.WarehouseId,
                TotalAmount = order.TotalAmount,
                TotalDiscount = order.TotalDiscount,
                TotalTax = order.TotalTax,
                TotalSurcharge = order.TotalSurcharge,
                Items = order.OrderItems?.Select(oi => new OrderItemResponse
                {
                    ItemUid = oi.ItemUid,
                    Amount = oi.Amount
                }).ToList()
            };
        }


    }
}
