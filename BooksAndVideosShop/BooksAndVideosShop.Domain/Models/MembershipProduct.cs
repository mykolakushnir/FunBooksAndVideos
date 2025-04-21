using BooksAndVideosShop.Domain.Enums;

namespace BooksAndVideosShop.Domain.Models
{
    public class MembershipProduct
    {
        public byte Id { get; set; }

        public string Name { get; set; }

        public MembershipType MembershipType { get; set; } = MembershipType.None;
    }
}