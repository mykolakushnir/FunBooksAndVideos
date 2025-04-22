using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Domain.Enums;
using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;
using BooksAndVideosShop.Logic.BusinessRules;
using BooksAndVideosShop.Logic.Services;
using BooksAndVideosShop.Logic.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BooksAndVideosShop.Logic.Tests
{
    [TestFixture]
    public class PurchaseOrderProcessorTests
    {
        [Test]
        public async Task Should_Add_Order_When_Customer_Exists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PurchaseOrderProcessor>>();
            var dbContext = TestDbContextFactory.Create();
            var businessRules = new List<IBusinessRule>();

            dbContext.Customers.Add(new Customer { Id = 1 });
            await dbContext.SaveChangesAsync();

            var orderProcessor = new PurchaseOrderProcessor(dbContext, loggerMock.Object, businessRules);

            var orderDto = new PurchaseOrderDto {
                CustomerId = 1,
                Total = 50.00m,
                OrderItemLines = new List<OrderItemLineDto>()
            };

            // Act
            await orderProcessor.ProcessAsync(orderDto);

            // Assert
            dbContext.PurchaseOrders.Should().ContainSingle();
            dbContext.PurchaseOrders.First().Total.Should().Be(50.00m);
        }

        [Test]
        public async Task Should_Use_Bankers_Rounding_For_Total()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PurchaseOrderProcessor>>();
            var dbContext = TestDbContextFactory.Create();
            dbContext.Customers.Add(new Customer { Id = 1 });
            await dbContext.SaveChangesAsync();

            var orderProcessor = new PurchaseOrderProcessor(dbContext, loggerMock.Object, []);

            var orderDto = new PurchaseOrderDto {
                CustomerId = 1,
                Total = 10.555m // should round to 10.56 (Banker's Rounding)
            };

            // Act
            await orderProcessor.ProcessAsync(orderDto);

            // Assert
            dbContext.PurchaseOrders.First().Total.Should().Be(10.56m);
        }

        [Test]
        public async Task Should_Apply_All_BusinessRules()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PurchaseOrderProcessor>>();
            var loggerMembershipRuleMock = new Mock<ILogger<CustomerMembershipRule>>();
            var loggerPhysicalProductRuleMock = new Mock<ILogger<PhysicalProductRule>>();
            var dbContext = TestDbContextFactory.Create();

            var membershipRule = new CustomerMembershipRule(loggerMembershipRuleMock.Object);
            var physicalProductRule = new PhysicalProductRule(loggerPhysicalProductRuleMock.Object);

            var customer = new Customer { Id = 1, ActiveMembership = MembershipType.None };
            var membership = new MembershipProduct { Id = 1, MembershipType = MembershipType.VideoClub, Name = "Video Club" };
            var product = new PhysicalProduct { Id = 100, Name = "Test Book", ProductType = ProductType.Book };

            dbContext.Customers.Add(customer);
            dbContext.MembershipProducts.Add(membership);
            dbContext.PhysicalProducts.Add(product);
            await dbContext.SaveChangesAsync();

            var orderProcessor = new PurchaseOrderProcessor(dbContext, loggerMock.Object, new List<IBusinessRule> { membershipRule, physicalProductRule });

            var orderDto = new PurchaseOrderDto {
                CustomerId = 1,
                CustomerMembershipId = 1,
                Total = 100m,
                OrderItemLines = new List<OrderItemLineDto>()
                {
                    new() { PhysicalProductId = product.Id }
                }
            };

            // Act
            await orderProcessor.ProcessAsync(orderDto);

            // Assert
            customer.ActiveMembership.Should().Be(MembershipType.VideoClub);
            dbContext.PurchaseOrders.Should().ContainSingle();
            dbContext.PurchaseOrders.First().OrderItemLines.Should().ContainSingle();
        }

        [Test]
        public async Task Should_Fail_When_Customer_Not_Found()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PurchaseOrderProcessor>>();
            var dbContext = TestDbContextFactory.Create();
            var orderProcessor = new PurchaseOrderProcessor(dbContext, loggerMock.Object, []);

            var orderDto = new PurchaseOrderDto {
                CustomerId = 999, // does not exist
                Total = 20.00m,
                OrderItemLines = new List<OrderItemLineDto>()
            };

            // Act
            var orderResult = await orderProcessor.ProcessAsync(orderDto);

            // Assert            
            orderResult.Error.Should().Be("Customer 999 not found.");
        }

        [Test]
        public async Task Should_Assign_Membership_If_Present()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PurchaseOrderProcessor>>();
            var dbContext = TestDbContextFactory.Create();

            var customer = new Customer { Id = 1 };
            var membership = new MembershipProduct { Id = 1, Name = "Book Club", MembershipType = MembershipType.BookClub };

            dbContext.Customers.Add(customer);
            dbContext.MembershipProducts.Add(membership);
            await dbContext.SaveChangesAsync();

            var orderProcessor = new PurchaseOrderProcessor(dbContext, loggerMock.Object, []);

            var orderDto = new PurchaseOrderDto {
                CustomerId = 1,
                CustomerMembershipId = 1,
                Total = 20m
            };

            // Act
            await orderProcessor.ProcessAsync(orderDto);

            // Assert
            var saved = dbContext.PurchaseOrders.First();
            saved.CustomerMembershipId.Should().Be(1);
            saved.CustomerMembership.Should().NotBeNull();
            saved.CustomerMembership.MembershipType.Should().Be(MembershipType.BookClub);
        }
    }
}