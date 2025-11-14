namespace CloudCart.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Navigation properties
    public ShoppingCart ShoppingCart { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    private CartItem() { }

    public static CartItem Create(Guid cartId, Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        return new CartItem
        {
            CartId = cartId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Quantity = quantity;
        MarkAsUpdated();
    }

    public decimal GetTotalPrice()
    {
        return Quantity * UnitPrice;
    }
}
