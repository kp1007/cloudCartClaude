using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudCart.Infrastructure.Data.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCarts");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.CreatedAt)
            .IsRequired();

        builder.Property(sc => sc.UpdatedAt);

        builder.Property(sc => sc.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(sc => sc.CartItems)
            .WithOne(ci => ci.ShoppingCart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
