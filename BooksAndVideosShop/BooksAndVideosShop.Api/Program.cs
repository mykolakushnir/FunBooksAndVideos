
using BooksAndVideosShop.DataAccess.Context;
using BooksAndVideosShop.DataAccess.Helpers;
using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Logic.BusinessRules;
using BooksAndVideosShop.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BooksAndVideosShop.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // setup logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                "Logs/log-.txt", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = ConfigurationHelper.LoadConfiguration();
            var connectionString = configuration.GetConnectionString("DevConnection");

            builder.Services.AddDbContext<ShopDbContext>(options => options.UseSqlite(connectionString));

            builder.Services.AddScoped<IBusinessRule, CustomerMembershipRule>();
            builder.Services.AddScoped<IBusinessRule, PhysicalProductRule>();
            builder.Services.AddScoped<IPurchaseOrderProcessor, PurchaseOrderProcessor>();
            builder.Services.AddScoped<IDbSeeder, DbSeeder>();

            try
            {
                Log.Information("Starting BooksAndVideosShop application");

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();

                    using (var scope = app.Services.CreateScope())
                    {
                        var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
                        await seeder.SeedTestDataAsync();
                    }
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Something went wrong!");
            }
            finally 
            {
                await Log.CloseAndFlushAsync();
            }            
        }
    }
}
