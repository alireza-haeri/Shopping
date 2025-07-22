using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.User;

namespace Shopping.Infrastructure.Persistence.Configurations.User;

internal class UserTokenEntityConfiguration:IEntityTypeConfiguration<UserTokenEntity>
{
    public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
    {
        builder.Property(c => c.Value);
        builder.Property(c => c.LoginProvider).HasMaxLength(450);
        builder.Property(c => c.Name).HasMaxLength(450);

        builder.HasKey(c => new { c.UserId, c.LoginProvider, c.Name });

        builder.ToTable("UserTokens", "user");
    }
}