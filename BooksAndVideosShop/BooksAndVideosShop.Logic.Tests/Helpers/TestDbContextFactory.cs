using BooksAndVideosShop.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace BooksAndVideosShop.Logic.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static ShopDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            return new ShopDbContext(options);
        }
    }
}
