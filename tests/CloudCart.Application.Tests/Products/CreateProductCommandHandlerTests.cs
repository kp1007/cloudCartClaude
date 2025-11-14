using CloudCart.API.Features.Products.CreateProduct;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;
using CloudCart.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CloudCart.Application.Tests.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateProductCommandHandler(_productRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_ShouldReturnSuccess()
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

        var command = new CreateProductCommand(request);

        _productRepositoryMock
            .Setup(x => x.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        var savedProduct = Product.Create("TEST-001", "Test Product", "Test Description", 99.99m, 100, request.CategoryId, "/test.jpg");
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedProduct);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SKU.Should().Be("TEST-001");
        result.Value.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateSKU_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            SKU = "EXISTING-SKU",
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100,
            CategoryId = Guid.NewGuid(),
            ImageUrl = "/test.jpg"
        };

        var command = new CreateProductCommand(request);

        var existingProduct = Product.Create("EXISTING-SKU", "Existing Product", "Description", 50m, 10, Guid.NewGuid(), "/test.jpg");

        _productRepositoryMock
            .Setup(x => x.GetBySkuAsync("EXISTING-SKU", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }
}
