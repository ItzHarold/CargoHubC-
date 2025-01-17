using Backend.Features.Orders;
using System.Linq;
using Xunit;
using Backend.Features.Items;
using Backend.UnitTests.Factories;

namespace Backend.Features.Orders.Tests
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderService(InMemoryDatabaseFactory.CreateMockContext());
        }

        [Fact]
        public void GetAllOrders_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _orderService.GetAllOrders();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddOrder_ValidOrder_IncreasesOrderCount()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "46578",
                OrderStatus = "Pending",
                WarehouseId = 1,
                ShipTo = "Client 1",
                BillTo = "Client 2",
                TotalAmount = 100,
                Items = [] 
            };

            // Act
            _orderService.AddOrder(order);
            var allOrders = _orderService.GetAllOrders();

            // Assert
            Assert.Single(allOrders);
            Assert.Equal(order.Reference, allOrders.First().Reference);
        }

        [Fact]
        public void GetOrderById_OrderExists_ReturnsOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "45678",
                OrderStatus = "Pending",
                WarehouseId = 1,
                ShipTo = "Client 1",
                BillTo = "Client 2",
                TotalAmount = 200,
                Items = [] 
            };
            _orderService.AddOrder(order);

            // Act
            var retrievedOrder = _orderService.GetOrderById(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(order.Reference, retrievedOrder?.Reference);
        }

        [Fact]
        public void GetOrderById_OrderDoesNotExist_ReturnsNull()
        {
            // Act
            var result = _orderService.GetOrderById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateOrder_OrderExists_UpdatesOrderData()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "123456",
                OrderStatus = "Pending",
                WarehouseId = 1,
                ShipTo = "Client 1",
                BillTo = "Client 2",
                TotalAmount = 150,
                Items = [] 
            };
            _orderService.AddOrder(order);

            var updatedOrder = new Order
            {
                Id = order.Id,
                SourceId = order.SourceId,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(5),
                Reference = "Updated reference",
                OrderStatus = "Shipped",
                WarehouseId = order.WarehouseId,
                ShipTo = order.ShipTo,
                BillTo = order.BillTo,
                TotalAmount = 175,
                Items = order.Items
            };

            // Act
            _orderService.UpdateOrder(updatedOrder);
            var retrievedOrder = _orderService.GetOrderById(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(updatedOrder.Reference, retrievedOrder?.Reference);
            Assert.Equal(updatedOrder.TotalAmount, retrievedOrder?.TotalAmount);
            Assert.Equal(updatedOrder.OrderStatus, retrievedOrder?.OrderStatus);
        }

        [Fact]
        public void DeleteOrder_OrderExists_RemovesOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 2,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "12345",
                OrderStatus = "Pending",
                WarehouseId = 1,
                ShipTo = "Client 1",
                BillTo = "Client 2",
                TotalAmount = 300,
                Items = [] 
            };
            _orderService.AddOrder(order);

            // Act
            _orderService.DeleteOrder(order.Id);
            var result = _orderService.GetAllOrders();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteOrder_OrderDoesNotExist_NoChangesMade()
        {
            // Arrange
            var order = new Order
            {
                Id = 3,
                SourceId = 1,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "12345",
                OrderStatus = "Pending",
                WarehouseId = 1,
                ShipTo = "Client 1",
                BillTo = "Client 2",
                TotalAmount = 400,
                Items = [] 
            };
            _orderService.AddOrder(order);

            // Act
            _orderService.DeleteOrder(999);

            // Assert
            Assert.Single(_orderService.GetAllOrders());
        }
    }
}
