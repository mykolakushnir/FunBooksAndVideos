using Microsoft.Extensions.Configuration;

namespace BooksAndVideosShop.DataAccess.Helpers
{
    public class ConfigurationHelper
    {
        public static string GetConnectionString()
        {
            var configuration = LoadConfiguration();
            return configuration.GetConnectionString("DevConnection") ?? string.Empty;
        }

        public static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}