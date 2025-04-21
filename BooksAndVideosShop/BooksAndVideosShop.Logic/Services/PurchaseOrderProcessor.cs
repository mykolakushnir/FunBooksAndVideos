using BooksAndVideosShop.DataAccess.Context;
using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using BooksAndVideosShop.Domain.Helpers;

namespace BooksAndVideosShop.Logic.Services
{
    public class PurchaseOrderProcessor : IPurchaseOrderProcessor
    {
        private readonly ShopDbContext _dbContext;
        private readonly List<IBusinessRule> _businessRules;
        

        public PurchaseOrderProcessor(ShopDbContext dbContext, IEnumerable<IBusinessRule> businessRules)
        {
            _dbContext = dbContext;
            _businessRules = businessRules.ToList();
        }

        public async Task ProcessAsync(PurchaseOrderDto orderDto)
        {
            var customer = await _dbContext.Customers.FindAsync(orderDto.CustomerId);

            if (customer == null)
            {
                throw new Exception($"Customer {orderDto.CustomerId} not found.");
            }

            MembershipProduct? membership = null;
            if (orderDto.CustomerMembershipId.HasValue)
            {
                membership = await _dbContext.MembershipProducts.FindAsync(orderDto.CustomerMembershipId.Value);

                if (membership == null) 
                {
                    throw new Exception($"Customer Membership {orderDto.CustomerMembershipId} not found.");
                }
            }

            var physicalProductIds = orderDto.OrderItemLines
                .Select(p => p.PhysicalProductId)
                .Distinct()
                .ToList();

            var physicalProducts = await _dbContext.PhysicalProducts
                .Where(p => physicalProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var orderItemLines = orderDto.OrderItemLines
                .Select(line => new OrderItemLine 
                {
                    PhysicalProductId = line.PhysicalProductId,
                    PhysicalProduct = physicalProducts[line.PhysicalProductId]
                })
                .ToList();

            // build order entity
            var order = new PurchaseOrder {
                CustomerId = orderDto.CustomerId,
                Customer = customer,

                CustomerMembershipId = orderDto.CustomerMembershipId,
                CustomerMembership = membership,

                Total = CurrencyHelper.ToBankersRounding(orderDto.Total),

                OrderItemLines = orderItemLines
            };


            foreach (var rule in _businessRules)
            {
                rule.Apply(order, customer);
            }

            await _dbContext.PurchaseOrders.AddAsync(order);
            _dbContext.Customers.Update(customer);

            await _dbContext.SaveChangesAsync();
        }
    }
}