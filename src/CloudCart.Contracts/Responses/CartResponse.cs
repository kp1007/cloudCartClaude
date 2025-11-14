namespace CloudCart.Contracts.Responses;

public class CartResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<CartItemResponse> CartItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
