using CloudCart.API.Features.Products.CreateProduct;
using CloudCart.Contracts.Requests;
using FluentAssertions;
using Xunit;

namespace CloudCart.Application.Tests.Products;

public class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator;

    public CreateProductValidatorTests()
    {
        _validator = new CreateProductValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            SKU = "TEST-001",
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100,
            CategoryId = Guid.NewGuid(),
            ImageUrl = "/test.jpg"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySKU_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            SKU = "",
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100,
            CategoryId = Guid.NewGuid(),
            ImageUrl = "/test.jpg"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SKU");
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            SKU = "TEST-001",
            Name = "Test Product",
            Description = "Test Description",
            Price = -10m,
            StockQuantity = 100,
            CategoryId = Guid.NewGuid(),
            ImageUrl = "/test.jpg"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }
}
