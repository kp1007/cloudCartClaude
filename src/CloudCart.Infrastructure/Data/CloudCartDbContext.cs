using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCart.Infrastructure.Data;

public class CloudCartDbContext : DbContext
{
    public CloudCartDbContext(DbContextOptions<CloudCartDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CloudCartDbContext).Assembly);

        // Global query filter for soft delete
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<ShoppingCart>().HasQueryFilter(sc => !sc.IsDeleted);

        // Seed data
        SeedData(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Ensure UTC for all DateTime values
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Modified)
                {
                    baseEntity.MarkAsUpdated();
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        var categories = new[]
        {
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Electronics", Description = "Electronic devices and accessories", ParentCategoryId = (Guid?)null, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Clothing", Description = "Fashion and apparel", ParentCategoryId = (Guid?)null, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Books", Description = "Books and reading materials", ParentCategoryId = (Guid?)null, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Home & Garden", Description = "Home improvement and garden supplies", ParentCategoryId = (Guid?)null, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Sports", Description = "Sports equipment and accessories", ParentCategoryId = (Guid?)null, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false }
        };

        modelBuilder.Entity<Category>().HasData(categories);

        // Seed Products
        var products = new[]
        {
            new { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), SKU = "ELEC-001", Name = "Laptop Pro 15", Description = "High-performance laptop", Price = 1299.99m, StockQuantity = 50, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ImageUrl = "/images/laptop.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), SKU = "ELEC-002", Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 29.99m, StockQuantity = 200, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ImageUrl = "/images/mouse.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"), SKU = "ELEC-003", Name = "USB-C Hub", Description = "7-in-1 USB-C Hub", Price = 49.99m, StockQuantity = 150, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ImageUrl = "/images/usb-hub.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"), SKU = "ELEC-004", Name = "Bluetooth Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, StockQuantity = 80, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ImageUrl = "/images/headphones.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"), SKU = "CLTH-001", Name = "Cotton T-Shirt", Description = "100% cotton t-shirt", Price = 19.99m, StockQuantity = 300, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), ImageUrl = "/images/tshirt.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"), SKU = "CLTH-002", Name = "Denim Jeans", Description = "Classic blue jeans", Price = 59.99m, StockQuantity = 120, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), ImageUrl = "/images/jeans.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"), SKU = "CLTH-003", Name = "Winter Jacket", Description = "Warm winter jacket", Price = 129.99m, StockQuantity = 60, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), ImageUrl = "/images/jacket.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("b4444444-4444-4444-4444-444444444444"), SKU = "CLTH-004", Name = "Running Shoes", Description = "Comfortable running shoes", Price = 89.99m, StockQuantity = 100, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), ImageUrl = "/images/shoes.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"), SKU = "BOOK-001", Name = "Clean Code", Description = "A Handbook of Agile Software Craftsmanship", Price = 39.99m, StockQuantity = 75, CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"), ImageUrl = "/images/cleancode.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"), SKU = "BOOK-002", Name = "Design Patterns", Description = "Elements of Reusable Object-Oriented Software", Price = 44.99m, StockQuantity = 50, CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"), ImageUrl = "/images/designpatterns.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("c3333333-3333-3333-3333-333333333333"), SKU = "BOOK-003", Name = "The Pragmatic Programmer", Description = "Your Journey to Mastery", Price = 42.99m, StockQuantity = 60, CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"), ImageUrl = "/images/pragmatic.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("c4444444-4444-4444-4444-444444444444"), SKU = "BOOK-004", Name = "Domain-Driven Design", Description = "Tackling Complexity in the Heart of Software", Price = 49.99m, StockQuantity = 40, CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"), ImageUrl = "/images/ddd.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"), SKU = "HOME-001", Name = "LED Desk Lamp", Description = "Adjustable LED desk lamp", Price = 34.99m, StockQuantity = 90, CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444"), ImageUrl = "/images/lamp.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"), SKU = "HOME-002", Name = "Garden Tools Set", Description = "Complete gardening tool set", Price = 79.99m, StockQuantity = 45, CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444"), ImageUrl = "/images/tools.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"), SKU = "HOME-003", Name = "Storage Bins", Description = "Stackable storage bins", Price = 24.99m, StockQuantity = 200, CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444"), ImageUrl = "/images/bins.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"), SKU = "HOME-004", Name = "Wall Clock", Description = "Modern wall clock", Price = 29.99m, StockQuantity = 70, CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444"), ImageUrl = "/images/clock.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("e1111111-1111-1111-1111-111111111111"), SKU = "SPRT-001", Name = "Yoga Mat", Description = "Non-slip yoga mat", Price = 24.99m, StockQuantity = 150, CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"), ImageUrl = "/images/yogamat.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("e2222222-2222-2222-2222-222222222222"), SKU = "SPRT-002", Name = "Dumbbells Set", Description = "Adjustable dumbbells", Price = 99.99m, StockQuantity = 55, CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"), ImageUrl = "/images/dumbbells.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("e3333333-3333-3333-3333-333333333333"), SKU = "SPRT-003", Name = "Resistance Bands", Description = "Set of 5 resistance bands", Price = 19.99m, StockQuantity = 180, CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"), ImageUrl = "/images/bands.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("e4444444-4444-4444-4444-444444444444"), SKU = "SPRT-004", Name = "Water Bottle", Description = "Insulated water bottle", Price = 14.99m, StockQuantity = 250, CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"), ImageUrl = "/images/bottle.jpg", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false }
        };

        modelBuilder.Entity<Product>().HasData(products);

        // Seed Customers with owned Email entity
        modelBuilder.Entity<Customer>().HasData(
            new { Id = Guid.Parse("c0000001-0000-0000-0000-000000000001"), FirstName = "John", LastName = "Doe", PhoneNumber = "+1234567890", CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false },
            new { Id = Guid.Parse("c0000002-0000-0000-0000-000000000002"), FirstName = "Jane", LastName = "Smith", PhoneNumber = "+1234567891", CreatedAt = DateTime.UtcNow, UpdatedAt = (DateTime?)null, IsDeleted = false }
        );

        // Seed Email owned entities for Customers
        modelBuilder.Entity<Customer>().OwnsOne(c => c.Email).HasData(
            new { CustomerId = Guid.Parse("c0000001-0000-0000-0000-000000000001"), Value = "john.doe@example.com" },
            new { CustomerId = Guid.Parse("c0000002-0000-0000-0000-000000000002"), Value = "jane.smith@example.com" }
        );
    }
}
