using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;

namespace BooksAndVideosShop.Logic.BusinessRules
{
    public class PhysicalProductRule : IBusinessRule
    {
        public void Apply(PurchaseOrder order, Customer customer)
        {
            if (!order.OrderItemLines.Any())
            {
                return;
            }

            var productNames = order.OrderItemLines
                .Select(x => x.PhysicalProduct.Name)
                .ToList();

            foreach (var name in productNames)
            {
                Console.WriteLine($"Generated shipping slip for Customer: {order.CustomerId} for Product: {name}");
            }
        }
    }
}