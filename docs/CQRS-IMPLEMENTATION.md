# CQRS Implementation Guide

## What is CQRS?

**CQRS** (Command Query Responsibility Segregation) separates read operations (queries) from write operations (commands).

### Core Principles

1. **Commands**: Mutate state, don't return data
2. **Queries**: Return data, don't mutate state
3. **Handlers**: Process commands and queries
4. **Single Responsibility**: Each handler does one thing

## Why Custom CQRS?

Most .NET projects use **MediatR** for CQRS, but we built our own implementation. Here's why:

### Advantages of Custom Implementation

| Aspect | Custom CQRS | MediatR |
|--------|-------------|---------|
| **Explicitness** | All dependencies visible in DI | Hidden behind IMediator |
| **Performance** | Direct method calls | Reflection + pipeline overhead |
| **Learning** | Understand CQRS deeply | Black box abstraction |
| **Control** | Full control over behavior | Limited to MediatR features |
| **Debugging** | Simple stack traces | Complex pipeline traces |
| **Dependencies** | Zero external dependencies | Requires MediatR package |

### Disadvantages

- More boilerplate in DI registration
- No built-in pipeline behaviors
- Must implement cross-cutting concerns manually

## Core Abstractions

### 1. Command Interface

```csharp
/// <summary>
/// Marker interface for commands that modify state
/// </summary>
public interface ICommand
{
}
```

**Purpose**: Tag interface to identify commands.

**Example**:
```csharp
public class CreateProductCommand : ICommand
{
    public CreateProductRequest Request { get; }

    public CreateProductCommand(CreateProductRequest request)
    {
        Request = request;
    }
}
```

### 2. Command Handler Interface

```csharp
/// <summary>
/// Handler for commands that modify state
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken ct = default);
}

/// <summary>
/// Handler for commands that modify state and return data
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken ct = default);
}
```

**Two Variants**:
- **Without Response**: For updates, deletes
- **With Response**: For creates (return created entity)

### 3. Query Interface

```csharp
/// <summary>
/// Marker interface for queries that retrieve data
/// </summary>
public interface IQuery<TResponse>
{
}
```

**Example**:
```csharp
public class GetProductsQuery : IQuery<PagedResult<ProductResponse>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetProductsQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
```

### 4. Query Handler Interface

```csharp
/// <summary>
/// Handler for queries that retrieve data
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken ct = default);
}
```

## Result Pattern

### Why Result Pattern?

Instead of throwing exceptions for business logic failures, we return `Result` objects:

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error) { ... }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }
}
```

### Benefits

1. **Type Safety**: Compiler forces error handling
2. **Clarity**: Explicit success/failure states
3. **No Exceptions**: Better performance
4. **Testability**: Easy to assert on results

### Usage Example

```csharp
public async Task<Result<ProductResponse>> HandleAsync(
    CreateProductCommand command,
    CancellationToken ct)
{
    // Validation failure
    if (productExists)
        return Result.Failure<ProductResponse>("Product already exists");

    // Business logic
    var product = CreateProduct(command);
    await _repository.AddAsync(product);

    // Success
    return Result.Success(MapToResponse(product));
}
```

## Complete Implementation Example

### Step 1: Create Command

```csharp
// CloudCart.API/Features/Products/CreateProduct/CreateProductCommand.cs
public class CreateProductCommand : ICommand
{
    public CreateProductRequest Request { get; }

    public CreateProductCommand(CreateProductRequest request)
    {
        Request = request;
    }
}
```

### Step 2: Create Handler

```csharp
// CloudCart.API/Features/Products/CreateProduct/CreateProductCommandHandler.cs
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponse>> HandleAsync(
        CreateProductCommand command,
        CancellationToken ct = default)
    {
        // Check for duplicates
        var existing = await _repository.GetBySkuAsync(command.Request.SKU, ct);
        if (existing != null)
            return Result.Failure<ProductResponse>("SKU already exists");

        // Create entity
        var product = ProductMapper.ToEntity(command.Request);

        // Persist
        await _repository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Reload with relationships
        var saved = await _repository.GetByIdAsync(product.Id, ct);

        // Return response
        return Result.Success(ProductMapper.ToResponse(saved!));
    }
}
```

### Step 3: Create Validator

```csharp
// CloudCart.API/Features/Products/CreateProduct/CreateProductValidator.cs
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
```

### Step 4: Register in DI

```csharp
// Program.cs
builder.Services.AddScoped<
    ICommandHandler<CreateProductCommand, ProductResponse>,
    CreateProductCommandHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
```

### Step 5: Use in Controller

```csharp
// CloudCart.API/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICommandHandler<CreateProductCommand, ProductResponse> _handler;

    public ProductsController(
        ICommandHandler<CreateProductCommand, ProductResponse> handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var command = new CreateProductCommand(request);
        var result = await _handler.HandleAsync(command, ct);

        if (result.IsFailure)
            return BadRequest(ApiResponse<ProductResponse>.FailureResponse(result.Error));

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Value.Id },
            ApiResponse<ProductResponse>.SuccessResponse(result.Value));
    }
}
```

## Query Example

### Create Query

```csharp
public class GetProductsQuery : IQuery<PagedResult<ProductResponse>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetProductsQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
```

### Create Query Handler

```csharp
public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<ProductResponse>>> HandleAsync(
        GetProductsQuery query,
        CancellationToken ct = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var products = await _repository.GetActiveProductsAsync(skip, query.PageSize, ct);
        var totalCount = await _repository.GetActiveProductsCountAsync(ct);

        var result = new PagedResult<ProductResponse>
        {
            Items = ProductMapper.ToResponseList(products),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return Result.Success(result);
    }
}
```

## Handler Registration Patterns

### Option 1: Explicit Registration (Current Approach)

```csharp
// Explicit - every handler registered individually
builder.Services.AddScoped<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>, GetProductsQueryHandler>();
```

**Pros**: Crystal clear, easy to debug
**Cons**: Verbose, manual work

### Option 2: Assembly Scanning (Alternative)

```csharp
// Scan assembly and register all handlers automatically
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

**Pros**: Less boilerplate
**Cons**: Magic, harder to debug

## Benefits of Our Implementation

### 1. Explicit Dependencies

Controller dependencies are clear:
```csharp
public ProductsController(
    ICommandHandler<CreateProductCommand, ProductResponse> createHandler,
    IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>> getHandler)
```

vs. MediatR:
```csharp
public ProductsController(IMediator mediator) // What can this do?
```

### 2. No Reflection Overhead

Direct method call:
```csharp
var result = await _handler.HandleAsync(command);
```

vs. MediatR:
```csharp
var result = await _mediator.Send(command); // Reflection to find handler
```

### 3. Easy Testing

Mock the exact handler you need:
```csharp
var handlerMock = new Mock<ICommandHandler<CreateProductCommand, ProductResponse>>();
```

### 4. Clear Stack Traces

Our stack trace:
```
ProductsController.CreateProduct
  → CreateProductCommandHandler.HandleAsync
    → ProductRepository.AddAsync
```

MediatR stack trace:
```
ProductsController.CreateProduct
  → Mediator.Send
    → PipelineBehavior.Handle
      → RequestHandler.Handle
        → CreateProductCommandHandler.HandleAsync
```

## Trade-offs

### When to Use Custom CQRS

✅ Small to medium projects
✅ Learning CQRS fundamentals
✅ Performance-critical applications
✅ Explicit dependencies preferred

### When to Use MediatR

✅ Large projects with many handlers
✅ Need pipeline behaviors (logging, validation)
✅ Team familiar with MediatR
✅ Want convention over configuration

## Testing Handlers

### Unit Test Example

```csharp
public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new CreateProductCommandHandler(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateProductCommand(new CreateProductRequest { ... });

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
```

## Conclusion

Our custom CQRS implementation provides:

- **Simplicity**: Easy to understand and debug
- **Performance**: No reflection overhead
- **Explicitness**: Clear dependencies
- **Flexibility**: Full control

While it requires more setup than MediatR, the benefits of clarity and simplicity make it ideal for learning and many production scenarios.
