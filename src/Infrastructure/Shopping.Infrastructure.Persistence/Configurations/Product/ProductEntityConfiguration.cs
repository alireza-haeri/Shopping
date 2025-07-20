using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shopping.Domain.Entities.Product;

namespace Shopping.Infrastructure.Persistence.Configurations.Product;

internal class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("Products", "product");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).HasMaxLength(100).IsRequired().IsUnicode();
        builder.Property(p => p.Description).HasMaxLength(2048).IsRequired().IsUnicode();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired().IsUnicode(false);
        builder.Property(p => p.Quantity).IsRequired().IsUnicode(false);
        builder.Property(p => p.State).HasConversion<EnumToStringConverter<ProductEntity.ProductState>>()
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasIndex(p => p.Title);
        builder.HasIndex(p => new { p.Title, p.State, p.Price, p.Quantity });

        builder.OwnsMany(p => p.Images, navigationBuilder => { navigationBuilder.ToJson("Images"); });
        builder.OwnsMany(p => p.ChangeLogs, navigationBuilder => { navigationBuilder.ToJson("ChangeLogs"); });

        builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
        builder.HasOne(p => p.User).WithMany(u => u.Products).HasForeignKey(p => p.UserId);
    }
}