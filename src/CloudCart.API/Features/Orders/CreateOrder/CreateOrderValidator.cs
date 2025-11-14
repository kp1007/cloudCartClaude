using CloudCart.Contracts.Requests;
using FluentValidation;

namespace CloudCart.API.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required");

        RuleFor(x => x.ShippingAddress.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

        RuleFor(x => x.ShippingAddress.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.ShippingAddress.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");

        RuleFor(x => x.ShippingAddress.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .MaximumLength(20).WithMessage("Zip code must not exceed 20 characters");

        RuleFor(x => x.ShippingAddress.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        });
    }
}
