using BooksAndVideosShop.Domain.Models;

namespace BooksAndVideosShop.Domain.Interfaces
{
    public interface IBusinessRule
    {
        void Apply(PurchaseOrder order, Customer customer);
    }
}