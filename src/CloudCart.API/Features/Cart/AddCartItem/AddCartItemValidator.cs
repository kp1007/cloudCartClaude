using CloudCart.Contracts.Requests;
using FluentValidation;

namespace CloudCart.API.Features.Cart.AddCartItem;

public class AddCartItemValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");
    }
}
