using FluentValidation;
using Backend.Infrastructure.Database;

namespace Backend.Features.ItemTypes
{
    public class ItemTypeValidator : AbstractValidator<ItemType>
    {
        private readonly CargoHubDbContext _dbContext;

        public ItemTypeValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(itemType => itemType.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(itemType => itemType.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
