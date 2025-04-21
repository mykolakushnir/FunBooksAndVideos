using BooksAndVideosShop.Domain.Enums;

namespace BooksAndVideosShop.Domain.Models
{
    public class PhysicalProduct
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public ProductType ProductType { get; set; }       
    }
}