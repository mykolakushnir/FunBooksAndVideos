using BooksAndVideosShop.DataAccess.Context;
using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using BooksAndVideosShop.Domain.Helpers;
using Microsoft.Extensions.Logging;
using BooksAndVideosShop.Domain.DTOs.Responses;
using BooksAndVideosShop.Logic.Results;

namespace BooksAndVideosShop.Logic.Services
{
    public class PurchaseOrderProcessor : IPurchaseOrderProcessor
    {
        private readonly ShopDbContext _dbContext;
        private readonly ILogger<PurchaseOrderProcessor> _logger;
        private readonly List<IBusinessRule> _businessRules;
        

        public PurchaseOrderProcessor(ShopDbContext dbContext, ILogger<PurchaseOrderProcessor> logger, IEnumerable<IBusinessRule> businessRules)
        {
            _dbContext = dbContext;
            _logger = logger;
            _businessRules = businessRules.ToList();
        }

        public async Task<Result<PurchaseOrderResponseDto>> ProcessAsync(PurchaseOrderDto orderDto)
        {
            var customer = await _dbContext.Customers.FindAsync(orderDto.CustomerId);

            if (customer == null)
            {
                var error = $"Customer {orderDto.CustomerId} not found.";
                _logger.LogWarning(error);
                return Result<PurchaseOrderResponseDto>.Failure(error);
            }

            MembershipProduct? membership = null;
            if (orderDto.CustomerMembershipId.HasValue)
            {
                membership = await _dbContext.MembershipProducts.FindAsync(orderDto.CustomerMembershipId.Value);

                if (membership == null) 
                {
                    var error = $"Customer Membership {orderDto.CustomerMembershipId} not found.";
                    _logger.LogWarning(error);
                    return Result<PurchaseOrderResponseDto>.Failure(error);
                }
            }

            var physicalProductIds = orderDto.OrderItemLines
                .Select(p => p.PhysicalProductId)
                .Distinct()
                .ToList();

            var physicalProducts = await _dbContext.PhysicalProducts
                .Where(p => physicalProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var missingIds = physicalProductIds.Except(physicalProducts.Keys).ToList();
            if (missingIds.Any())
            {
                var error = $"Some of Physical products not found: {string.Join(", ", missingIds)}";
                _logger.LogWarning(error);
                return Result<PurchaseOrderResponseDto>.Failure(error);
            }

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

            _logger.LogInformation("Purchase Order saved. OrderId: {OrderId}, CustomerId: {CustomerId}", order.Id, order.CustomerId);

            return Result<PurchaseOrderResponseDto>.Success(new PurchaseOrderResponseDto 
            {
                Id = order.Id
            });
        }
    }
}