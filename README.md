# CloudCart - E-Commerce Platform

A production-ready e-commerce platform built with .NET 9, implementing Clean Architecture with Vertical Slices, custom CQRS pattern (without MediatR), and manual mapping (without AutoMapper).

## Technology Stack

- **.NET 9** - ASP.NET Core Web API
- **Entity Framework Core 9** - Code-First with SQL Server
- **FluentValidation** - Request validation pipeline
- **Serilog** - Structured logging
- **xUnit + FluentAssertions** - Testing framework

## Key Features

- Custom CQRS implementation (no MediatR)
- Manual mapping with static mapper classes (no AutoMapper)
- Vertical Slice Architecture
- Result pattern for error handling
- Repository pattern with Unit of Work
- Soft delete with global query filters
- FluentValidation pipeline
- Comprehensive test coverage

## Quick Start

### Prerequisites

- .NET 9 SDK
- SQL Server or SQL Server LocalDB

### 5-Minute Setup

```bash
# 1. Clone the repository
git clone <repository-url>
cd CloudCart

# 2. Restore NuGet packages
dotnet restore

# 3. Update database connection string in appsettings.json
# (Default uses LocalDB)

# 4. Run EF migrations
dotnet ef database update -p src/CloudCart.Infrastructure -s src/CloudCart.API

# 5. Run the application
dotnet run --project src/CloudCart.API
```

The API will be available at `https://localhost:7001` (or as configured).

Swagger UI: `https://localhost:7001/swagger`

## Project Structure

```
CloudCart/
├── src/
│   ├── CloudCart.API/          # Web API layer with vertical slices
│   │   ├── Features/           # Feature folders (Products, Orders, Cart, Categories)
│   │   ├── Controllers/        # API controllers
│   │   └── Middleware/         # Exception handling, validation
│   ├── CloudCart.Domain/       # Domain entities, value objects, enums
│   ├── CloudCart.Application/  # CQRS abstractions, mappers
│   ├── CloudCart.Infrastructure/ # EF Core, repositories
│   └── CloudCart.Contracts/    # Request/Response DTOs
├── tests/
│   ├── CloudCart.Application.Tests/    # Unit tests
│   └── CloudCart.Integration.Tests/    # Integration tests
└── docs/                       # Documentation
```

## API Endpoints

### Products
- `POST /api/products` - Create product
- `GET /api/products` - Get products (paginated)
- `GET /api/products/{id}` - Get product by ID
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product (soft delete)

### Orders
- `POST /api/orders` - Create order
- `GET /api/orders?customerId={id}` - Get customer's orders
- `GET /api/orders/{id}` - Get order by ID
- `PUT /api/orders/{id}/status` - Update order status
- `POST /api/orders/{id}/cancel` - Cancel order

### Cart
- `GET /api/cart?customerId={id}` - Get customer's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{itemId}` - Update cart item quantity
- `DELETE /api/cart/items/{itemId}` - Remove cart item
- `DELETE /api/cart?customerId={id}` - Clear cart
- `POST /api/cart/checkout` - Checkout cart (creates order)

### Categories
- `GET /api/categories` - Get all categories (hierarchical)
- `GET /api/categories/{id}` - Get category with products

## Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/CloudCart.Application.Tests

# Run integration tests only
dotnet test tests/CloudCart.Integration.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Documentation

- [Architecture Overview](docs/ARCHITECTURE.md) - System design and architecture decisions
- [CQRS Implementation](docs/CQRS-IMPLEMENTATION.md) - Custom CQRS pattern explained
- [Azure Setup Guide](docs/AZURE-SETUP.md) - Deploying to Azure

## Design Decisions

### Why No MediatR?

- **Simplicity**: Direct handler registration makes the DI container explicit
- **Performance**: Eliminates reflection overhead
- **Clarity**: Clear dependencies between controllers and handlers
- **Learning**: Understanding CQRS fundamentals without abstraction

### Why No AutoMapper?

- **Explicitness**: Every mapping is visible and intentional
- **Performance**: No reflection or runtime overhead
- **Debugging**: Easy to trace and debug mappings
- **Control**: Full control over complex mapping scenarios

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For issues and questions, please open an issue on GitHub.
