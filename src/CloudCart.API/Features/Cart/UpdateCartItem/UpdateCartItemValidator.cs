using CloudCart.Contracts.Requests;
using FluentValidation;

namespace CloudCart.API.Features.Cart.UpdateCartItem;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");
    }
}
