namespace BooksAndVideosShop.Domain.DTOs
{
    public class PurchaseOrderDto
    {
        public long CustomerId { get; set; }
        public decimal Total { get; set; }
        public byte? CustomerMembershipId { get; set; }

        public List<OrderItemLineDto> OrderItemLines { get; set; } = [];

    }
}