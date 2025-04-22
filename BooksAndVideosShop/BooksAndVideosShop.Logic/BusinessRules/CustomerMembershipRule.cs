using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BooksAndVideosShop.Logic.BusinessRules
{
    public class CustomerMembershipRule : IBusinessRule
    {
        private readonly ILogger<CustomerMembershipRule> _logger;

        public CustomerMembershipRule(ILogger<CustomerMembershipRule> logger)
        {
            _logger = logger;
        }

        public void Apply(PurchaseOrder order, Customer customer)
        {
            if (order.CustomerMembership != null)
            {
                customer.ActiveMembership = order.CustomerMembership.MembershipType;
                _logger.LogInformation($"Activated {customer.ActiveMembership} membership for customer {customer.Id}");
            }
        }
    }
}