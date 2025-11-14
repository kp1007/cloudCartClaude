namespace CloudCart.Contracts.Responses;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public List<CategoryResponse> SubCategories { get; set; } = new();
    public List<ProductResponse> Products { get; set; } = new();
}
