namespace CloudCart.Contracts.Requests;

public class AddCartItemRequest
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
