using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Product;

namespace Shopping.Infrastructure.Persistence.Configurations.Category;

internal class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("Categories", "product");
        
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Title);
        
        builder.Property(c => c.Title).HasMaxLength(100).IsRequired().IsUnicode();
        builder.Property(c => c.ParentId).IsRequired(false).IsUnicode(false);
        
        builder.HasMany(c=>c.Products).WithOne(c=>c.Category);
    }
}