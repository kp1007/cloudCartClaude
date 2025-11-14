using CloudCart.API.Features.Cart.AddCartItem;
using CloudCart.API.Features.Cart.CheckoutCart;
using CloudCart.API.Features.Cart.ClearCart;
using CloudCart.API.Features.Cart.GetCart;
using CloudCart.API.Features.Cart.RemoveCartItem;
using CloudCart.API.Features.Cart.UpdateCartItem;
using CloudCart.API.Features.Categories.GetCategories;
using CloudCart.API.Features.Categories.GetCategoryById;
using CloudCart.API.Features.Orders.CancelOrder;
using CloudCart.API.Features.Orders.CreateOrder;
using CloudCart.API.Features.Orders.GetOrderById;
using CloudCart.API.Features.Orders.GetOrders;
using CloudCart.API.Features.Orders.UpdateOrderStatus;
using CloudCart.API.Features.Products.CreateProduct;
using CloudCart.API.Features.Products.DeleteProduct;
using CloudCart.API.Features.Products.GetProductById;
using CloudCart.API.Features.Products.GetProducts;
using CloudCart.API.Features.Products.UpdateProduct;
using CloudCart.API.Middleware;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;
using CloudCart.Infrastructure.Data;
using CloudCart.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<CloudCartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

// Product Handlers
builder.Services.AddScoped<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>, GetProductsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateProductCommand>, UpdateProductCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteProductCommand>, DeleteProductCommandHandler>();

// Order Handlers
builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, OrderResponse>, CreateOrderCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrdersQuery, List<OrderResponse>>, GetOrdersQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderResponse>, GetOrderByIdQueryHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateOrderStatusCommand>, UpdateOrderStatusCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CancelOrderCommand>, CancelOrderCommandHandler>();

// Cart Handlers
builder.Services.AddScoped<IQueryHandler<GetCartQuery, CartResponse>, GetCartQueryHandler>();
builder.Services.AddScoped<ICommandHandler<AddCartItemCommand, CartResponse>, AddCartItemCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCartItemCommand>, UpdateCartItemCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RemoveCartItemCommand>, RemoveCartItemCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ClearCartCommand>, ClearCartCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CheckoutCartCommand, OrderResponse>, CheckoutCartCommandHandler>();

// Category Handlers
builder.Services.AddScoped<IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>, GetCategoriesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetCategoryByIdQuery, CategoryResponse>, GetCategoryByIdQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
