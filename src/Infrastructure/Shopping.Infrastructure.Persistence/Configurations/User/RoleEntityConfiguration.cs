using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.User;

namespace Shopping.Infrastructure.Persistence.Configurations.User;

internal class RoleEntityConfiguration:IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.DisplayName).HasMaxLength(100);
        builder.Property(c => c.NormalizedName).HasMaxLength(100);
        builder.Property(c => c.ConcurrencyStamp).HasMaxLength(100);

        builder.HasMany(c => c.RoleClaims)
            .WithOne(c => c.Role)
            .HasForeignKey(c => c.RoleId);

        builder.HasMany(c => c.UserRoles)
            .WithOne(c => c.Role)
            .HasForeignKey(c => c.RoleId);

        builder.HasIndex(c => c.NormalizedName);

        builder.ToTable("Roles", "user");
    }
}