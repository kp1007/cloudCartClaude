using CloudCart.Contracts.Requests;
using FluentValidation;

namespace CloudCart.API.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeAValidStatus).WithMessage("Invalid order status");
    }

    private bool BeAValidStatus(string status)
    {
        var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled", "Refunded" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
