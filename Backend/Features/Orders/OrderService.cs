using Backend.Infrastructure.Database;
using Backend.Features.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using Backend.Features.OrderItems;
using Backend.Request;
using Backend.Response;
using Backend.Features.ShipmentOrders;

namespace Backend.Features.Orders
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders(
            string? sort,
            string? direction,
            string? reference,
            float? totalAmount,
            float? totalDiscount,
            float? totalTax,
            float? totalSurcharge,
            string? orderStatus,
            int? warehouseId);
        Order? GetOrderById(int id);
        Task<int> AddOrder(OrderRequest orderRequest);
        Task UpdateOrder(int orderId, OrderRequest orderRequest);
        void DeleteOrder(int id);
        IEnumerable<Item> GetItemsByOrderId(int orderId);
        void UpdateItemInOrder(int orderId, string itemUid, Item updatedItem);
        OrderResponse MapToResponse(Order order);
        IEnumerable<Order> GetOrdersByClientId(int clientId);

    }

    public class OrderService : IOrderService
    {
        private readonly CargoHubDbContext _dbContext;

        public OrderService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> AddOrder(OrderRequest orderRequest)
        {
            if (_dbContext.Warehouses == null)
            {
                throw new InvalidOperationException("Warehouses DbSet is not initialized");
            }

            if (_dbContext.Contacts == null)
            {
                throw new InvalidOperationException("Contacts DbSet is not initialized");
            }

            if (_dbContext.Clients == null)
            {
                throw new InvalidOperationException("Clients DbSet is not initialized");
            }

            if (_dbContext.Orders == null)
            {
                throw new InvalidOperationException("Orders DbSet is not initialized");
            }

            if (!_dbContext.Warehouses.Any(warehouse => warehouse.Id == orderRequest.WarehouseId))
            {
                throw new ArgumentException("Warehouse not found");
            }

            if (!_dbContext.Contacts.Any(contact => contact.Id == orderRequest.SourceId))
            {
                throw new ArgumentException("Contact not found sourceId");
            }

            if (orderRequest.ShipTo.HasValue && !_dbContext.Clients.Any(client => client.Id == orderRequest.ShipTo))
            {
                throw new ArgumentException("Client not found for ShipTo");
            }

            if (orderRequest.BillTo.HasValue && !_dbContext.Clients.Any(client => client.Id == orderRequest.BillTo))
            {
                throw new ArgumentException("Client not found for BillTo");
            }

            if (orderRequest.ShipmentId.HasValue)
            {
                if (_dbContext.Shipments == null)
                {
                    throw new InvalidOperationException("Shipments DbSet is not initialized");
                }

                if (!_dbContext.Shipments.Any(s => s.Id == orderRequest.ShipmentId))
                {
                    throw new ArgumentException("Shipment not found");
                }
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
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Orders.Add(order);

            // Add order items if provided
            if (orderRequest.Items != null && orderRequest.Items.Count != 0)
            {
                order.OrderItems = orderRequest.Items.Select(i => new OrderItem
                {
                    ItemUid = i.ItemUid,
                    Amount = i.Amount,
                    OrderId = order.Id
                }).ToList();
            }

            await _dbContext.SaveChangesAsync();

            // Create shipment-order relationship if shipment ID is provided
            if (orderRequest.ShipmentId.HasValue)
            {
                if (_dbContext.ShipmentOrders == null)
                {
                    throw new InvalidOperationException("ShipmentOrders DbSet is not initialized");
                }

                var shipmentOrder = new ShipmentOrder
                {
                    ShipmentId = orderRequest.ShipmentId.Value,
                    OrderId = order.Id
                };
                _dbContext.ShipmentOrders.Add(shipmentOrder);
                await _dbContext.SaveChangesAsync();
            }

            return order.Id;
        }

        public IEnumerable<Order> GetAllOrders(
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
            if (_dbContext.Orders == null)
            {
                return new List<Order>();
            }

            IQueryable<Order> query = _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShipmentOrders)
                .AsQueryable();

            if (!string.IsNullOrEmpty(reference))
            {
                query = query.Where(o => o.Reference != null && o.Reference.Contains(reference));
            }

            if (totalAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount == totalAmount);
            }

            if (totalDiscount.HasValue)
            {
                query = query.Where(o => o.TotalDiscount == totalDiscount);
            }

            if (totalTax.HasValue)
            {
                query = query.Where(o => o.TotalTax == totalTax);
            }

            if (totalSurcharge.HasValue)
            {
                query = query.Where(o => o.TotalSurcharge == totalSurcharge);
            }

            if (!string.IsNullOrEmpty(orderStatus))
            {
                query = query.Where(o => o.OrderStatus != null && o.OrderStatus.Contains(orderStatus));
            }

            if (warehouseId.HasValue)
            {
                query = query.Where(o => o.WarehouseId == warehouseId);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "reference":
                        query = direction == "desc" ? query.OrderByDescending(o => o.Reference) : query.OrderBy(o => o.Reference);
                        break;
                    case "totalamount":
                        query = direction == "desc" ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount);
                        break;
                    case "totaldiscount":
                        query = direction == "desc" ? query.OrderByDescending(o => o.TotalDiscount) : query.OrderBy(o => o.TotalDiscount);
                        break;
                    case "totaltax":
                        query = direction == "desc" ? query.OrderByDescending(o => o.TotalTax) : query.OrderBy(o => o.TotalTax);
                        break;
                    case "totalsurcharge":
                        query = direction == "desc" ? query.OrderByDescending(o => o.TotalSurcharge) : query.OrderBy(o => o.TotalSurcharge);
                        break;
                    case "orderstatus":
                        query = direction == "desc" ? query.OrderByDescending(o => o.OrderStatus) : query.OrderBy(o => o.OrderStatus);
                        break;
                    case "warehouseid":
                        query = direction == "desc" ? query.OrderByDescending(o => o.WarehouseId) : query.OrderBy(o => o.WarehouseId);
                        break;
                    default:
                        query = query.OrderBy(o => o.Reference);
                        break;
                }
            }

            return query.ToList();
        }

        public Order? GetOrderById(int id)
        {
            if (_dbContext.Orders == null)
            {
                return null;
            }

            return _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShipmentOrders)
                .FirstOrDefault(o => o.Id == id);
        }

        public async Task UpdateOrder(int orderId, OrderRequest orderRequest)
        {
            if (_dbContext.Orders == null)
            {
                throw new InvalidOperationException("Orders DbSet is not initialized");
            }

            var order = _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShipmentOrders)
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

            // Handle shipment relationship
            if (orderRequest.ShipmentId.HasValue)
            {
                if (_dbContext.Shipments == null)
                {
                    throw new InvalidOperationException("Shipments DbSet is not initialized");
                }

                if (!_dbContext.Shipments.Any(s => s.Id == orderRequest.ShipmentId))
                {
                    throw new ArgumentException("Shipment not found");
                }

                if (_dbContext.ShipmentOrders == null)
                {
                    throw new InvalidOperationException("ShipmentOrders DbSet is not initialized");
                }

                // Check if the relationship already exists
                var existingShipmentOrder = order.ShipmentOrders
                    .FirstOrDefault(so => so.ShipmentId == orderRequest.ShipmentId.Value);

                if (existingShipmentOrder == null)
                {
                    // Create new relationship
                    var shipmentOrder = new ShipmentOrder
                    {
                        ShipmentId = orderRequest.ShipmentId.Value,
                        OrderId = order.Id
                    };
                    _dbContext.ShipmentOrders.Add(shipmentOrder);
                }
            }
            else
            {
                if (_dbContext.ShipmentOrders == null)
                {
                    throw new InvalidOperationException("ShipmentOrders DbSet is not initialized");
                }

                // Remove existing shipment relationships
                var existingShipmentOrders = order.ShipmentOrders.ToList();
                foreach (var shipmentOrder in existingShipmentOrders)
                {
                    _dbContext.ShipmentOrders.Remove(shipmentOrder);
                }
            }

            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteOrder(int id)
        {
            if (_dbContext.Orders == null)
            {
                throw new InvalidOperationException("Orders DbSet is not initialized");
            }

            if (_dbContext.ShipmentOrders == null)
            {
                throw new InvalidOperationException("ShipmentOrders DbSet is not initialized");
            }

            if (_dbContext.OrderItems == null)
            {
                throw new InvalidOperationException("OrderItems DbSet is not initialized");
            }

            var order = _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShipmentOrders)
                .FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                // Remove shipment orders first
                if (order.ShipmentOrders != null)
                {
                    _dbContext.ShipmentOrders.RemoveRange(order.ShipmentOrders);
                }

                // Remove order items
                if (order.OrderItems != null)
                {
                    _dbContext.OrderItems.RemoveRange(order.OrderItems);
                }

                // Remove the order
                _dbContext.Orders.Remove(order);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Item> GetItemsByOrderId(int orderId)
        {
            if (_dbContext.Orders == null)
            {
                return Enumerable.Empty<Item>();
            }

            var order = _dbContext.Orders
                .Include(o => o.OrderItems)
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
                throw new InvalidOperationException("Orders DbSet is not initialized");
            }

            if (_dbContext.Items == null)
            {
                throw new InvalidOperationException("Items DbSet is not initialized");
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

            var item = order.OrderItems.FirstOrDefault(i => i.Item != null && i.Item.Uid == itemUid);

            if (item == null)
            {
                throw new ArgumentException($"Item with UID {itemUid} not found in order {orderId}.");
            }

            _dbContext.SaveChanges();
        }

        public OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                RequestDate = order.RequestDate,
                Reference = order.Reference,
                ReferenceExtra = order.ReferenceExtra,
                OrderStatus = order.OrderStatus,
                Notes = order.Notes,
                ShippingNotes = order.ShippingNotes,
                PickingNotes = order.PickingNotes,
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

        public IEnumerable<Order> GetOrdersByClientId(int clientId)
        {
            if (_dbContext.Orders == null)
            {
                return Enumerable.Empty<Order>();
            }

            var orders = _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShipmentOrders)
                .Where(o => o.BillToClientId == clientId || o.ShipToClientId == clientId)
                .ToList();

            return orders;
        }

    }
}
