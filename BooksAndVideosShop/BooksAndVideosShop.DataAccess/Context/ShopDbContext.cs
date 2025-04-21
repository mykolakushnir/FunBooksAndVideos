using BooksAndVideosShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksAndVideosShop.DataAccess.Context
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }
                
        public DbSet<Customer> Customers { get; set; }

        public DbSet<PhysicalProduct> PhysicalProducts { get; set; }

        public DbSet<MembershipProduct> MembershipProducts { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<OrderItemLine> OrderItemLines { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer
            modelBuilder.Entity<Customer>(entity => 
            {
                entity.Property(e => e.Id);

                entity.Property(e => e.ActiveMembership)
                    .HasColumnType("int")
                    .IsRequired();
            });

            // PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>(entity => 
            {
                entity.Property(e => e.Id);

                entity.Property(e => e.CustomerId);

                entity.Property(e => e.CustomerMembershipId);

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .HasConstraintName("FK__PurchaseOrders_CustomerId__Customers_Id")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CustomerMembership)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerMembershipId)
                    .HasConstraintName("FK__PurchaseOrders_CustomerMembershipId__MembershipProducts_Id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.OrderItemLines)
                    .WithOne(e => e.PurchaseOrder)
                    .HasForeignKey(x => x.PurchaseOrderId)
                    .HasConstraintName("FK__OrderItemLines_PurchaseOrderId__PurchaseOrders_Id")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItemLine
            modelBuilder.Entity<OrderItemLine>(entity => {
                entity.Property(e => e.Id);

                entity.Property(e => e.PurchaseOrderId);

                entity.Property(e => e.PhysicalProductId);

                entity.HasOne(e => e.PhysicalProduct)
                    .WithMany()
                    .HasForeignKey(x => x.PhysicalProductId)
                    .HasConstraintName("FK__OrderItemLines_PhysicalProductId__PhysicalProducts_Id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PhysicalProduct
            modelBuilder.Entity<PhysicalProduct>(entity => {
                entity.Property(e => e.Id);

                entity.Property(e => e.Name)
                    .HasColumnType("nvarchar(200)")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.ProductType)
                    .HasColumnType("int")
                    .IsRequired();
            });

            // MembershipProduct
            modelBuilder.Entity<MembershipProduct>(entity => {
                entity.Property(e => e.Id);

                entity.Property(e => e.Name)
                    .HasColumnType("nvarchar(200)")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.MembershipType)
                    .HasColumnType("int")
                    .IsRequired();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}