namespace CloudCart.Domain.Entities;

public class Product : BaseEntity
{
    public string SKU { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public Guid CategoryId { get; private set; }
    public string ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public Category Category { get; private set; } = null!;

    private Product()
    {
        SKU = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        ImageUrl = string.Empty;
    }

    public static Product Create(
        string sku,
        string name,
        string description,
        decimal price,
        int stockQuantity,
        Guid categoryId,
        string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty", nameof(sku));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        return new Product
        {
            SKU = sku,
            Name = name,
            Description = description ?? string.Empty,
            Price = price,
            StockQuantity = stockQuantity,
            CategoryId = categoryId,
            ImageUrl = imageUrl ?? string.Empty,
            IsActive = true
        };
    }

    public void Update(string name, string description, decimal price, int stockQuantity, Guid categoryId, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        Name = name;
        Description = description ?? string.Empty;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        ImageUrl = imageUrl ?? string.Empty;
        MarkAsUpdated();
    }

    public void UpdateStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new InvalidOperationException("Insufficient stock");

        StockQuantity += quantity;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}
