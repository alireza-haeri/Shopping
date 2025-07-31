using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Cart;
using Shopping.Domain.Entities.User; // Required for the relationship

namespace Shopping.Infrastructure.Persistence.Configurations.Cart;

public class CartEntityConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("Carts", "cart");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();
        
        builder.HasIndex(c => c.UserId)
            .IsUnique();

        builder.HasOne<UserEntity>()
            .WithOne() 
            .HasForeignKey<CartEntity>(c => c.UserId)
            .IsRequired();

         builder.HasMany(c => c.Items)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .IsRequired(); 

        builder.Ignore(c => c.TotalPrice);
    }
}