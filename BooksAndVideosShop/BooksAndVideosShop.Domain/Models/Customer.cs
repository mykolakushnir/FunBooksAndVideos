using BooksAndVideosShop.Domain.Enums;

namespace BooksAndVideosShop.Domain.Models
{
    public class Customer
    {
        public long Id { get; set; }

        public MembershipType ActiveMembership { get; set; } = MembershipType.None;
    }
}