namespace BooksAndVideosShop.DataAccess.Helpers
{
    public interface IDbSeeder
    {
        Task SeedTestDataAsync(CancellationToken cancellationToken = default);
    }
}