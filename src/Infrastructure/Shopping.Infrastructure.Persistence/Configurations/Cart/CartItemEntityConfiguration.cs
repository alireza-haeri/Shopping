using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Cart;
using Shopping.Domain.Entities.Product;

namespace Shopping.Infrastructure.Persistence.Configurations.Cart;

public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        builder.ToTable("CartItems", "cart");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}