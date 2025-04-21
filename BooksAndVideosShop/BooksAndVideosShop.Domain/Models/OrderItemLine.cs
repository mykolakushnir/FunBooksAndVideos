namespace BooksAndVideosShop.Domain.Models
{
    public class OrderItemLine
    {
        public long Id { get; set; }

        public long PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public long PhysicalProductId { get; set; }

        public PhysicalProduct PhysicalProduct { get; set; }
    }
}