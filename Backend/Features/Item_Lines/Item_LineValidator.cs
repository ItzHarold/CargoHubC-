using FluentValidation;
using Backend.Infrastructure.Database;

namespace Backend.Features.ItemLines
{
    public class ItemLineValidator : AbstractValidator<ItemLine>
    {
        private readonly CargoHubDbContext _dbContext;

        public ItemLineValidator(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(itemLine => itemLine.Name)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(itemLine => itemLine.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
