using CloudCart.Contracts.Requests;
using FluentValidation;

namespace CloudCart.API.Features.Products.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters");
    }
}
