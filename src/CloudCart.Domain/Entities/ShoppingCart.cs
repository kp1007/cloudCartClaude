namespace CloudCart.Domain.Entities;

public class ShoppingCart : BaseEntity
{
    public Guid CustomerId { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public ICollection<CartItem> CartItems { get; private set; }

    private ShoppingCart()
    {
        CartItems = new List<CartItem>();
    }

    public static ShoppingCart Create(Guid customerId)
    {
        return new ShoppingCart
        {
            CustomerId = customerId
        };
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        var existingItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var newItem = CartItem.Create(Id, productId, quantity, unitPrice);
            CartItems.Add(newItem);
        }

        MarkAsUpdated();
    }

    public void UpdateItem(Guid cartItemId, int quantity)
    {
        var item = CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (item == null)
            throw new InvalidOperationException("Cart item not found");

        item.UpdateQuantity(quantity);
        MarkAsUpdated();
    }

    public void RemoveItem(Guid cartItemId)
    {
        var item = CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (item != null)
        {
            CartItems.Remove(item);
            MarkAsUpdated();
        }
    }

    public void Clear()
    {
        CartItems.Clear();
        MarkAsUpdated();
    }

    public decimal GetTotalAmount()
    {
        return CartItems.Sum(item => item.GetTotalPrice());
    }
}
