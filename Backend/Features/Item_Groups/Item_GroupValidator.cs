using FluentValidation;
using Backend.Infrastructure.Database;

namespace Backend.Features.ItemGroups
{
    public class ItemGroupValidator : AbstractValidator<ItemGroup>
    {
        private readonly CargoHubDbContext _dbContext;

        public ItemGroupValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(itemGroup => itemGroup.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(itemGroup => itemGroup.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
