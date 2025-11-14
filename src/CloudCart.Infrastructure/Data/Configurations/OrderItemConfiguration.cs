using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudCart.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(oi => oi.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(oi => oi.CreatedAt)
            .IsRequired();

        builder.Property(oi => oi.UpdatedAt);

        builder.Property(oi => oi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
