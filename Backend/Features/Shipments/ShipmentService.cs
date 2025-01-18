using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Backend.Features.Items;
using Backend.Features.ShimpentItems;
using Backend.Infrastructure.Database;
using Backend.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Shipments
{
    public interface IShipmentService
    {
        IEnumerable<Shipment> GetAllShipments();
        Shipment? GetShipmentById(int id);
        Task<int> AddShipment(ShipmentRequest shipmentRequest);
        Task UpdateShipment(int id, ShipmentRequest request);
        void DeleteShipment(int id);
    }

    public class ShipmentService: IShipmentService
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly IValidator<Shipment> _validator;

        public ShipmentService(CargoHubDbContext dbContext, IValidator<Shipment> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public IEnumerable<Shipment> GetAllShipments()
        {
            if (_dbContext.Shipments != null)
            {
                return _dbContext.Shipments
                    .Include(s => s.ShipmentItems)
                    .Include(s => s.ShipmentOrders)
                    .ToList();
            }
            return new List<Shipment>();
        }

        public Shipment? GetShipmentById(int id)
        {
            return _dbContext?.Shipments
                ?.Include(s => s.ShipmentItems)
                ?.Include(s => s.SourceContact)
                ?.FirstOrDefault(s => s.Id == id);
        }

        public async Task<int> AddShipment(ShipmentRequest shipmentRequest)
        {
            var shipment = new Shipment()
            {
                SourceId = shipmentRequest.SourceId,
                OrderDate = shipmentRequest.OrderDate,
                RequestDate = shipmentRequest.RequestDate,
                ShipmentDate = shipmentRequest.ShipmentDate,
                ShipmentType = shipmentRequest.ShipmentType,
                ShipmentStatus = shipmentRequest.ShipmentStatus,
                Notes = shipmentRequest.Notes,
                CarrierCode = shipmentRequest.CarrierCode,
                CarrierDescription = shipmentRequest.CarrierDescription,
                ServiceCode = shipmentRequest.ServiceCode,
                PaymentType = shipmentRequest.PaymentType,
                TransferMode = shipmentRequest.TransferMode,
                TotalPackageCount = shipmentRequest.TotalPackageCount,
                TotalPackageWeight = shipmentRequest.TotalPackageWeight,
            };
            
            // Add shipment items if they exist
            if (shipmentRequest.ShipmentItems != null)
            {
                foreach (var item in shipmentRequest.ShipmentItems)
                {
                    var shipmentItem = new ShipmentItem
                    {
                        ItemUid = item.ItemId,
                        Amount = item.Amount,
                        ShipmentId = shipment.Id  // This will be set after SaveChanges
                    };
                    shipment.ShipmentItems.Add(shipmentItem);
                }
            }

            shipment.CreatedAt = DateTime.Now;
            shipment.UpdatedAt = shipment.CreatedAt;

            _dbContext.Shipments?.Add(shipment);
            await _dbContext.SaveChangesAsync();
            return shipment.Id;
        }

        
        public async Task UpdateShipment(int id, ShipmentRequest request)
        {
            var existingShipment = await _dbContext.Shipments!.FindAsync(id) 
                ?? throw new KeyNotFoundException($"Shipment with ID {id} not found.");

            // Update shipment properties
            existingShipment.SourceId = request.SourceId;
            existingShipment.OrderDate = request.OrderDate;
            existingShipment.ShipmentDate = request.ShipmentDate;
            existingShipment.ShipmentType = request.ShipmentType;
            existingShipment.ShipmentStatus = request.ShipmentStatus;
            existingShipment.Notes = request.Notes;
            existingShipment.CarrierCode = request.CarrierCode;
            existingShipment.CarrierDescription = request.CarrierDescription;
            existingShipment.ServiceCode = request.ServiceCode;
            existingShipment.PaymentType = request.PaymentType;
            existingShipment.TransferMode = request.TransferMode;
            existingShipment.TotalPackageCount = request.TotalPackageCount;
            existingShipment.TotalPackageWeight = request.TotalPackageWeight;
            existingShipment.UpdatedAt = DateTime.Now;


            var validationResult = _validator.Validate(existingShipment);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            _dbContext.Shipments?.Update(existingShipment);
            await _dbContext.SaveChangesAsync();
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
