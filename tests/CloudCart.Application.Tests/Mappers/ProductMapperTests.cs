using CloudCart.Application.Mappers;
using CloudCart.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CloudCart.Application.Tests.Mappers;

public class ProductMapperTests
{
    [Fact]
    public void ToResponse_ShouldMapProductCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var product = Product.Create(
            "TEST-001",
            "Test Product",
            "Test Description",
            99.99m,
            100,
            categoryId,
            "/test.jpg"
        );

        // Act
        var response = ProductMapper.ToResponse(product);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(product.Id);
        response.SKU.Should().Be("TEST-001");
        response.Name.Should().Be("Test Product");
        response.Description.Should().Be("Test Description");
        response.Price.Should().Be(99.99m);
        response.StockQuantity.Should().Be(100);
        response.CategoryId.Should().Be(categoryId);
        response.ImageUrl.Should().Be("/test.jpg");
        response.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToResponseList_ShouldMapMultipleProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("TEST-001", "Product 1", "Desc 1", 10m, 5, Guid.NewGuid(), "/1.jpg"),
            Product.Create("TEST-002", "Product 2", "Desc 2", 20m, 10, Guid.NewGuid(), "/2.jpg")
        };

        // Act
        var responses = ProductMapper.ToResponseList(products);

        // Assert
        responses.Should().HaveCount(2);
        responses[0].Name.Should().Be("Product 1");
        responses[1].Name.Should().Be("Product 2");
    }
}
