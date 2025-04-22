using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BooksAndVideosShop.Logic.BusinessRules
{
    public class PhysicalProductRule : IBusinessRule
    {
        private readonly ILogger<PhysicalProductRule> _logger;

        public PhysicalProductRule(ILogger<PhysicalProductRule> logger)
        {
            _logger = logger;
        }

        public void Apply(PurchaseOrder order, Customer customer)
        {
            if (!order.OrderItemLines.Any())
            {
                return;
            }

            // check If the purchase order contains physical product (Book)
            var productNames = order.OrderItemLines
                .Where(x => x.PhysicalProduct.ProductType == Domain.Enums.ProductType.Book)
                .Select(x => x.PhysicalProduct.Name)
                .ToList();
            
            foreach (var name in productNames)
            {
                _logger.LogInformation($"Generated shipping slip for Customer: {order.CustomerId} for next Physical Product: {name}");
            }
        }
    }
}