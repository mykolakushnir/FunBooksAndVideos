using BooksAndVideosShop.DataAccess.Context;
using BooksAndVideosShop.Domain.Enums;
using BooksAndVideosShop.Domain.Models;

namespace BooksAndVideosShop.DataAccess.Helpers
{
    public class DbSeeder : IDbSeeder
    {
        private readonly ShopDbContext _dbContext;

        public DbSeeder(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }       

        public async Task SeedTestDataAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

            if (_dbContext.Customers.Any())
            {
                return;
            }

            // Prepare customers data
            var customers = SeedCustomersData();
            _dbContext.Customers.AddRange(customers);
            
            // Prepare physical products data
            var physicalProducts = SeedPhysicalProductsData();
            _dbContext.PhysicalProducts.AddRange(physicalProducts);

            // Prepare membership products data
            var memberships = SeedMembershipProductsData();
            _dbContext.MembershipProducts.AddRange(memberships);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Prepare orders data
            var orders = SeedOrdersData(customers, physicalProducts);
            _dbContext.PurchaseOrders.AddRange(orders);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private List<Customer> SeedCustomersData()
        {
            return new List<Customer>() 
            {
                new() { ActiveMembership = MembershipType.None },
                new() { ActiveMembership = MembershipType.BookClub },
                new() { ActiveMembership = MembershipType.VideoClub },
                new() { ActiveMembership = MembershipType.Premium }
            };
        }

        private List<PhysicalProduct> SeedPhysicalProductsData()
        {
            var physicalProducts = new List<PhysicalProduct>();

            for (var i = 0; i < 10; i++)
            {
                physicalProducts.Add(new PhysicalProduct { Name = $"Test Book #{i + 1}", ProductType = ProductType.Book });
            }

            for (var i = 0; i < 5; i++)
            {
                physicalProducts.Add(new PhysicalProduct { Name = $"Test Video #{i + 1}", ProductType = ProductType.Video });
            }

            return physicalProducts;
        }

        private List<MembershipProduct> SeedMembershipProductsData()
        {
            return new List<MembershipProduct>()
            {
                new() { Id = 0, Name = "N/A", MembershipType = MembershipType.None },
                new() { Id = 1, Name = "Book Club", MembershipType = MembershipType.BookClub },
                new() { Id = 2, Name = "Video Club", MembershipType = MembershipType.VideoClub },
                new() { Id = 3, Name = "Premium", MembershipType = MembershipType.Premium }
            };
        }

        private List<PurchaseOrder> SeedOrdersData(List<Customer> customers, List<PhysicalProduct> physicalProducts)
        {
            return new List<PurchaseOrder>() 
            {
                new()
                {
                    CustomerId = customers[0].Id,
                    Total = 10.51m,
                    OrderItemLines = new List<OrderItemLine>
                    {
                        new() { PhysicalProductId = physicalProducts[0].Id },
                        new() { PhysicalProductId = physicalProducts[1].Id }
                    }
                },

                new()
                {
                    CustomerId = customers[1].Id,
                    CustomerMembershipId = 1,
                    Total = 25.18m,
                    OrderItemLines = new List<OrderItemLine>
                    {
                        new() { PhysicalProductId = physicalProducts[1].Id },
                        new() { PhysicalProductId = physicalProducts[3].Id },
                        new() { PhysicalProductId = physicalProducts[5].Id },
                        new() { PhysicalProductId = physicalProducts[7].Id }
                    }
                },

                new()
                {
                    CustomerId = customers[2].Id,
                    CustomerMembershipId = 2,
                    Total = 155.25m,
                    OrderItemLines = new List<OrderItemLine>
                    {
                        new() { PhysicalProductId = physicalProducts[10].Id },
                        new() { PhysicalProductId = physicalProducts[12].Id },
                        new() { PhysicalProductId = physicalProducts[14].Id }
                    }
                },

                new()
                {
                    CustomerId = customers[3].Id,
                    CustomerMembershipId = 3,
                    Total = 225.31m,
                    OrderItemLines = new List<OrderItemLine>
                    {
                        new() { PhysicalProductId = physicalProducts[0].Id },
                        new() { PhysicalProductId = physicalProducts[1].Id },
                        new() { PhysicalProductId = physicalProducts[2].Id },

                        new() { PhysicalProductId = physicalProducts[10].Id },
                        new() { PhysicalProductId = physicalProducts[11].Id },
                        new() { PhysicalProductId = physicalProducts[12].Id }
                    }
                }
            };
        }
    }
}