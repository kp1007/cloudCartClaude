using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.Application.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static List<ProductResponse> ToResponseList(IEnumerable<Product> products)
    {
        return products.Select(ToResponse).ToList();
    }

    public static Product ToEntity(CreateProductRequest request)
    {
        return Product.Create(
            request.SKU,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.CategoryId,
            request.ImageUrl
        );
    }

    public static void UpdateEntity(Product product, UpdateProductRequest request)
    {
        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.CategoryId,
            request.ImageUrl
        );
    }
}
