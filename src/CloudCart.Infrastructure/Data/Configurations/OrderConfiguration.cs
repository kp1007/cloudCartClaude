using CloudCart.Domain.Entities;
using CloudCart.Domain.Enums;
using CloudCart.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudCart.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Value object Address
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ShippingStreet");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingCity");

            address.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingState");

            address.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("ShippingZipCode");

            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingCountry");
        });

        // Relationships
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
