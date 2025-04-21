using BooksAndVideosShop.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BooksAndVideosShop.DataAccess.Context
{
    public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();
            var connectionString = ConfigurationHelper.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);

            return new ShopDbContext(optionsBuilder.Options);
        }

    }
}