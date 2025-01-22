using System.Linq;
using Backend.Features.Clients;
using Backend.Features.Contacts;
using Xunit;
using Backend.UnitTests.Factories;
using Backend.Features.Orders;
using Backend.Features.Warehouses;
using Backend.Infrastructure.Database;
using Backend.Request;


namespace Backend.Features.Orders.Tests
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;

        private readonly CargoHubDbContext _mockContext;

        public OrderServiceTests()
        {
            _mockContext = InMemoryDatabaseFactory.CreateMockContext();
            _orderService = new OrderService(_mockContext);

        }

        [Fact]
        public void GetAllOrders_InitiallyEmpty_ReturnsEmptyList()
        {
            // Act

            var result = _orderService.GetAllOrders(null, null, null, null, null, null, null, null, null);


            // Assert
            Assert.Empty(result);
        }

        [Fact]

        public async Task AddOrder_ValidOrder_IncreasesOrderCount()
        {
            // Arrange
            // Add required warehouse to the mock context
            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Main Warehouse",
                Address = "10 Wijnhaven",
                Zip = "1234JK",
                City = "Rotterdam",
                Province = "Zuid-Holland",
                Country = "Nederland"
            };
            _mockContext.Warehouses.Add(warehouse);

            // Add required source contact to the mock context
            var contact = new Contact
            {
                Id = 1,
                ContactName = "Source Contact",
                ContactEmail = "source@example.com",
                ContactPhone = "1234567890"
            };
            _mockContext.Contacts.Add(contact);

            // Add required clients to the mock context
            var shipToClient = new Client
            {
                Id = 1,
                Name = "Ship To Client",
                Address = "Client Address",
                City = "Rotterdam",
                ZipCode = "1234JK",
                Province = "Zuid-Holland",
                Country = "Nederland",
                ContactName = "Client Contact",
                ContactPhone = "1234567890",
                ContactEmail = "client@example.com"
            };
            _mockContext.Clients.Add(shipToClient);

            var billToClient = new Client
            {
                Id = 2,
                Name = "Bill To Client",
                Address = "Billing Address",
                City = "Amsterdam",
                ZipCode = "5678AB",
                Province = "Noord-Holland",
                Country = "Nederland",
                ContactName = "Billing Contact",
                ContactPhone = "9876543210",
                ContactEmail = "billing@example.com"
            };
            _mockContext.Clients.Add(billToClient);

            // Save changes to the mock context
            _mockContext.SaveChanges();

            var orderRequest = new OrderRequest
            {
                SourceId = contact.Id,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(2),
                Reference = "46578",
                OrderStatus = "Pending",
                WarehouseId = warehouse.Id,
                ShipTo = shipToClient.Id,
                BillTo = billToClient.Id,
                TotalAmount = 100,
                Items = new List<OrderItemRequest>()
            };

            // Act
            await _orderService.AddOrder(orderRequest);
            var allOrders = _orderService.GetAllOrders(null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.Single(allOrders);
            Assert.Contains(allOrders, o => o.Reference == orderRequest.Reference);
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
                ShipToClientId = 1,
                BillToClientId = 2,
                TotalAmount = 200
            };
            _mockContext.Orders.Add(order);
            _mockContext.SaveChanges();

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
        public async Task UpdateOrder_OrderExists_UpdatesOrderData()

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
                ShipToClientId = 1,
                BillToClientId = 2,
                TotalAmount = 150
            };
            _mockContext.Orders.Add(order);
            _mockContext.SaveChanges();

            var updatedOrderRequest = new OrderRequest
            {
                SourceId = (int)order.SourceId,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now.AddDays(5),
                Reference = "Updated reference",
                OrderStatus = "Shipped",
                WarehouseId = (int)order.WarehouseId,
                ShipTo = order.ShipToClientId,
                BillTo = order.BillToClientId,
                TotalAmount = 175
            };

            // Act
            await _orderService.UpdateOrder(order.Id, updatedOrderRequest);
            var retrievedOrder = _orderService.GetOrderById(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(updatedOrderRequest.Reference, retrievedOrder?.Reference);
            Assert.Equal(updatedOrderRequest.TotalAmount, retrievedOrder?.TotalAmount);
            Assert.Equal(updatedOrderRequest.OrderStatus, retrievedOrder?.OrderStatus);
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
                ShipToClientId = 1,
                BillToClientId = 2,
                TotalAmount = 300
            };
            _mockContext.Orders.Add(order);
            _mockContext.SaveChanges();

            // Act
            _orderService.DeleteOrder(order.Id);
            var result = _orderService.GetAllOrders(null, null, null, null, null, null, null, null, null);

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
                ShipToClientId = 1,
                BillToClientId = 2,
                TotalAmount = 400
            };
            _mockContext.Orders.Add(order);
            _mockContext.SaveChanges();

            // Act
            _orderService.DeleteOrder(999);

            // Assert
            Assert.Single(_orderService.GetAllOrders(null, null, null, null, null, null, null, null, null));

        }
    }
}
