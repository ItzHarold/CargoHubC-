using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Shipments
{
    public interface IShipmentService
    {
        IEnumerable<Shipment> GetAllShipments();
        Shipment? GetShipmentById(int id);
        void AddShipment(Shipment shipment);
        void UpdateShipment(Shipment shipment);
        void DeleteShipment(int id);
    }

    public class ShipmentService: IShipmentService
    {
        private readonly CargoHubDbContext _dbContext;

        public ShipmentService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Shipment> GetAllShipments()
        {
            if (_dbContext.Shipments != null)
            {
                return _dbContext.Shipments.ToList();
            }
            return new List<Shipment>();
        }
        public Shipment? GetShipmentById(int id)
        {
            return _dbContext.Shipments?.FirstOrDefault(s => s.Id == id);
        }
        public void AddShipment(Shipment shipment)
        {
            shipment.CreatedAt = DateTime.Now;
            _dbContext.Shipments?.Add(shipment);
            _dbContext.SaveChanges();
        }
        public void UpdateShipment(Shipment shipment)
        {
            shipment.UpdatedAt = DateTime.Now;
            _dbContext.Shipments?.Update(shipment);
            _dbContext.SaveChanges();
        }
        public void DeleteShipment(int id)
        {
            if (_dbContext.Shipments != null)
            {
                var shipment = _dbContext.Shipments
                    .FirstOrDefault(s => s.Id == id);

                if (shipment != null)
                {
                    _dbContext.Shipments?.Remove(shipment);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
