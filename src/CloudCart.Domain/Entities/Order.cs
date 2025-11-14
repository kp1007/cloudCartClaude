using CloudCart.Domain.Enums;
using CloudCart.Domain.ValueObjects;

namespace CloudCart.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string OrderNumber { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public ICollection<OrderItem> OrderItems { get; private set; }

    private Order()
    {
        OrderNumber = string.Empty;
        OrderItems = new List<OrderItem>();
    }

    public static Order Create(
        Guid customerId,
        Address shippingAddress,
        ICollection<OrderItem> orderItems)
    {
        if (orderItems == null || !orderItems.Any())
            throw new ArgumentException("Order must have at least one item", nameof(orderItems));

        var order = new Order
        {
            CustomerId = customerId,
            OrderNumber = GenerateOrderNumber(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = shippingAddress,
            OrderItems = orderItems
        };

        order.CalculateTotalAmount();

        return order;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = OrderItems.Sum(item => item.TotalPrice);
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        // Business rule: Cannot change status of cancelled orders
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot change status of a cancelled order");

        // Business rule: Cannot cancel delivered orders
        if (Status == OrderStatus.Delivered && newStatus == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        Status = newStatus;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
        MarkAsUpdated();
    }
}
