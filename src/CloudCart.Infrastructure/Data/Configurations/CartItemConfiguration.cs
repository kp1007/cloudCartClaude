using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudCart.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ci => ci.CreatedAt)
            .IsRequired();

        builder.Property(ci => ci.UpdatedAt);

        builder.Property(ci => ci.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
