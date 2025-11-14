namespace CloudCart.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    // Navigation properties
    public Category? ParentCategory { get; private set; }
    public ICollection<Category> SubCategories { get; private set; }
    public ICollection<Product> Products { get; private set; }

    private Category()
    {
        Name = string.Empty;
        Description = string.Empty;
        SubCategories = new List<Category>();
        Products = new List<Product>();
    }

    public static Category Create(string name, string description, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        return new Category
        {
            Name = name,
            Description = description ?? string.Empty,
            ParentCategoryId = parentCategoryId
        };
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        MarkAsUpdated();
    }
}
