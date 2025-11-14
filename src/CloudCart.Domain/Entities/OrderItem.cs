namespace CloudCart.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    // Navigation properties
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    private OrderItem() { }

    public static OrderItem Create(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        return new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice
        };
    }

    internal void SetOrderId(Guid orderId)
    {
        OrderId = orderId;
    }
}
