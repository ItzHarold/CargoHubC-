using FluentValidation;
using Backend.Infrastructure.Database;

namespace Backend.Features.Shipments
{
    public class ShipmentValidator : AbstractValidator<Shipment>
    {
        private readonly CargoHubDbContext _dbContext;

        public ShipmentValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
            
            RuleFor(shipment => shipment.SourceId)
                .GreaterThan(0).WithMessage("SourceId must be greater than 0.");

            RuleFor(shipment => shipment.OrderDate)
                .NotEmpty().WithMessage("OrderDate is required.");

            RuleFor(shipment => shipment.ShipmentType)
                .NotEmpty().WithMessage("ShipmentType is required.");

            RuleFor(shipment => shipment.CarrierCode)
                .NotEmpty().WithMessage("CarrierCode is required.");

            RuleFor(shipment => shipment.ServiceCode)
                .NotEmpty().WithMessage("ServiceCode is required.");

            RuleFor(shipment => shipment.PaymentType)
                .NotEmpty().WithMessage("PaymentType is required.");

            RuleFor(shipment => shipment.TransferMode)
                .NotEmpty().WithMessage("TransferMode is required.");

            RuleFor(shipment => shipment.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            RuleFor(shipment => shipment.TotalPackageCount)
                .GreaterThan(0).WithMessage("TotalPackageCount must be greater than 0.")
                .When(shipment => shipment.TotalPackageCount.HasValue);

            RuleFor(shipment => shipment.TotalPackageWeight)
                .GreaterThan(0).WithMessage("TotalPackageWeight must be greater than 0.")
                .When(shipment => shipment.TotalPackageWeight.HasValue);

            RuleForEach(shipment => shipment.ShipmentItems)
                .ChildRules(items =>
                {
                    items.RuleFor(item => item.Id)
                        .NotEmpty().WithMessage("ItemId is required.");

                    items.RuleFor(item => item.Amount)
                        .GreaterThan(0).WithMessage("Amount must be greater than 0.");
                });
        }
    }
}
