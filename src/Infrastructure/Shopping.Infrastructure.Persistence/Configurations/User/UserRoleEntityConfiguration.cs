using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.User;

namespace Shopping.Infrastructure.Persistence.Configurations.User;

internal class UserRoleEntityConfiguration:IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("UserRoles", "user");
        builder.HasKey(c => new { c.UserId, c.RoleId });
    }
}