using Backend.Infrastructure.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Warehouses
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        private readonly CargoHubDbContext _dbContext;
        private readonly bool _isUpdate;

        public WarehouseValidator(CargoHubDbContext dbContext, bool isUpdate = false)
        {
            _dbContext = dbContext;
            _isUpdate = isUpdate;

            RuleFor(warehouse => warehouse.Code)
                .NotNull().WithMessage("Code is required.")
                .NotEmpty().WithMessage("Code cannot be empty.");

            RuleFor(warehouse => warehouse.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(warehouse => warehouse.Address)
                .NotNull().WithMessage("Address is required.")
                .NotEmpty().WithMessage("Address cannot be empty.");

            RuleFor(warehouse => warehouse.Zip)
                .NotNull().WithMessage("Zip is required.")
                .NotEmpty().WithMessage("Zip cannot be empty.");

            RuleFor(warehouse => warehouse.City)
                .NotNull().WithMessage("City is required.")
                .NotEmpty().WithMessage("City cannot be empty.");

            RuleFor(warehouse => warehouse.Province)
                .NotNull().WithMessage("Province is required.")
                .NotEmpty().WithMessage("Province cannot be empty.");

            RuleFor(warehouse => warehouse.Country)
                .NotNull().WithMessage("Country is required.")
                .NotEmpty().WithMessage("Country cannot be empty.");

            if (!_isUpdate) // Skip validation for WarehouseContacts during updates
            {
                RuleFor(warehouse => warehouse.WarehouseContacts)
                    .NotNull().WithMessage("Contacts are required.")
                    .NotEmpty().WithMessage("Contacts Phone cannot be empty.");
            }
        }
    }

}
