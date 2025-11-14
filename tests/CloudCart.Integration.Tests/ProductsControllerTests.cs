using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CloudCart.Integration.Tests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ProductResponse>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            SKU = $"TEST-{Guid.NewGuid()}",
            Name = "Integration Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100,
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ImageUrl = "/test.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Integration Test Product");
    }
}
