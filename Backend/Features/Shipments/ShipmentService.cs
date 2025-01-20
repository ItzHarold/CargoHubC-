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
        IEnumerable<Shipment> GetAllShipments(
            string? sort,
            string? direction,
            int? sourceId,
            DateTime? orderDate,
            DateTime? requestDate,
            DateTime? shipmentDate,
            string? shipmentType,
            string? shipmentStatus,
            string? carrierCode,
            string? paymentType,
            string? transferMode,
            int? totalPackageCount,
            float? totalPackageWeight);
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

        public IEnumerable<Shipment> GetAllShipments(
            string? sort,
            string? direction,
            int? sourceId,
            DateTime? orderDate,
            DateTime? requestDate,
            DateTime? shipmentDate,
            string? shipmentType,
            string? shipmentStatus,
            string? carrierCode,
            string? paymentType,
            string? transferMode,
            int? totalPackageCount,
            float? totalPackageWeight)
        {
            if (_dbContext.Shipments == null)
            {
                return new List<Shipment>();
            }
            var query = _dbContext.Shipments
                .Include(s => s.ShipmentItems)
                .Include(s => s.ShipmentOrders)
                .AsQueryable();

            // Apply filtering based on the query parameters
            if (sourceId.HasValue)
            {
                query = query.Where(s => s.SourceId == sourceId);
            }

            if (orderDate.HasValue)
            {
                query = query.Where(s => s.OrderDate == orderDate);
            }

            if (requestDate.HasValue)
            {
                query = query.Where(s => s.RequestDate == requestDate);
            }

            if (shipmentDate.HasValue)
            {
                query = query.Where(s => s.ShipmentDate == shipmentDate);
            }

            if (!string.IsNullOrEmpty(shipmentType))
            {
                query = query.Where(s => s.ShipmentType.Contains(shipmentType));
            }

            if (!string.IsNullOrEmpty(shipmentStatus))
            {
                query = query.Where(s => s.ShipmentStatus!.Contains(shipmentStatus));
            }

            if (!string.IsNullOrEmpty(carrierCode))
            {
                query = query.Where(s => s.CarrierCode.Contains(carrierCode));
            }

            if (!string.IsNullOrEmpty(paymentType))
            {
                query = query.Where(s => s.PaymentType.Contains(paymentType));
            }

            if (!string.IsNullOrEmpty(transferMode))
            {
                query = query.Where(s => s.TransferMode.Contains(transferMode));
            }

            if (totalPackageCount.HasValue)
            {
                query = query.Where(s => s.TotalPackageCount == totalPackageCount);
            }

            if (totalPackageWeight.HasValue)
            {
                query = query.Where(s => s.TotalPackageWeight == totalPackageWeight);
            }

            // Apply sorting based on the sort and direction parameters
            if (!string.IsNullOrEmpty(sort))
            {
               switch (sort.ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case "source_id":
                        query = direction == "desc" ? query.OrderByDescending(s => s.SourceId) : query.OrderBy(s => s.SourceId);
                        break;
                    case "order_date":
                        query = direction == "desc" ? query.OrderByDescending(s => s.OrderDate) : query.OrderBy(s => s.OrderDate);
                        break;
                    case "request_date":
                        query = direction == "desc" ? query.OrderByDescending(s => s.RequestDate) : query.OrderBy(s => s.RequestDate);
                        break;
                    case "shipment_date":
                        query = direction == "desc" ? query.OrderByDescending(s => s.ShipmentDate) : query.OrderBy(s => s.ShipmentDate);
                        break;
                    case "shipment_type":
                        query = direction == "desc" ? query.OrderByDescending(s => s.ShipmentType) : query.OrderBy(s => s.ShipmentType);
                        break;
                    case "shipment_status":
                        query = direction == "desc" ? query.OrderByDescending(s => s.ShipmentStatus) : query.OrderBy(s => s.ShipmentStatus);
                        break;
                    case "carrier_code":
                        query = direction == "desc" ? query.OrderByDescending(s => s.CarrierCode) : query.OrderBy(s => s.CarrierCode);
                        break;
                    case "payment_type":
                        query = direction == "desc" ? query.OrderByDescending(s => s.PaymentType) : query.OrderBy(s => s.PaymentType);
                        break;
                    case "transfer_mode":
                        query = direction == "desc" ? query.OrderByDescending(s => s.TransferMode) : query.OrderBy(s => s.TransferMode);
                        break;
                    case "total_package_count":
                        query = direction == "desc" ? query.OrderByDescending(s => s.TotalPackageCount) : query.OrderBy(s => s.TotalPackageCount);
                        break;
                    case "total_package_weight":
                        query = direction == "desc" ? query.OrderByDescending(s => s.TotalPackageWeight) : query.OrderBy(s => s.TotalPackageWeight);
                        break;
                    default:
                        query = query.OrderBy(s => s.OrderDate); // Default sorting by `OrderDate`
                        break;
                }
            }

            // Execute and return the filtered and sorted query, including related entities
            return query.ToList();
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
