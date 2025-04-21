using BooksAndVideosShop.Domain.Interfaces;
using BooksAndVideosShop.Domain.Models;

namespace BooksAndVideosShop.Logic.BusinessRules
{
    public class CustomerMembershipRule : IBusinessRule
    {
        public void Apply(PurchaseOrder order, Customer customer)
        {
            if (order.CustomerMembership != null)
            {
                customer.ActiveMembership = order.CustomerMembership.MembershipType;
                Console.WriteLine($"Activated {customer.ActiveMembership} membership for customer {customer.Id}");
            }
        }
    }
}