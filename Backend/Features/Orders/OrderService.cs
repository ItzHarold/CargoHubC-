using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Backend.Features.Orders
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        IEnumerable<Order> GetOrdersFromCustomers(int id);
    }

    public class OrderService: IOrderService
    {
        private readonly List<Order> _orders = new();
        public void AddOrder(Order order)
        {
            _orders.Add(order);
        }

        public IEnumerable<Order> GetOrdersFromCustomers(int id)
        {
            return _orders.Where(o => o.SourceId == id);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orders;
        }
        public Order? GetOrderById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }
        public void UpdateOrder(Order order)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder == null)
            {
                return;
            }

            existingOrder.Id = order.Id;
            existingOrder.SourceId = order.SourceId;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.RequestDate = order.RequestDate;
            existingOrder.Reference = order.Reference;
            existingOrder.ReferenceExtra = order.ReferenceExtra;
            existingOrder.OrderStatus = order.OrderStatus;
            existingOrder.Notes = order.Notes;
            existingOrder.ShippingNotes = order.ShippingNotes;
            existingOrder.PickingNotes = order.PickingNotes;
            existingOrder.WarehouseId = order.WarehouseId;
            existingOrder.ShipTo = order.ShipTo;
            existingOrder.BillTo = order.BillTo;
            existingOrder.ShipmentId = order.ShipmentId;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.TotalDiscount = order.TotalDiscount;
            existingOrder.TotalTax = order.TotalTax;
            existingOrder.TotalSurcharge = order.TotalSurcharge;
            existingOrder.Items = order.Items;
        }
        public void DeleteOrder(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return;
            }

            _orders.Remove(order);
        }

    }
}
