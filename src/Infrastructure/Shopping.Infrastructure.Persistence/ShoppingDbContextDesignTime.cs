using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shopping.Infrastructure.Persistence;

public class ShoppingDbContextDesignTime : IDesignTimeDbContextFactory<ShoppingDbContext>
{
    public ShoppingDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer();
        
        return new ShoppingDbContext(optionBuilder.Options);
    }
}