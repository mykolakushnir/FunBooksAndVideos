namespace BooksAndVideosShop.Domain.Models
{
    public class PurchaseOrder
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public Customer Customer { get; set; }

        public decimal Total { get; set; }

        public List<OrderItemLine> OrderItemLines { get; set; } = [];

        public byte? CustomerMembershipId { get; set; }

        public MembershipProduct? CustomerMembership { get; set; }
    }
}