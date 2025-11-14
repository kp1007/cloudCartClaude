using CloudCart.Domain.Entities;
using CloudCart.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudCart.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Value object Email
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email");

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // Relationships
        builder.HasOne(c => c.ShoppingCart)
            .WithOne(sc => sc.Customer)
            .HasForeignKey<ShoppingCart>(sc => sc.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
