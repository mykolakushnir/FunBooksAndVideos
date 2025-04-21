using BooksAndVideosShop.Domain.DTOs;

namespace BooksAndVideosShop.Logic.Services
{
    public interface IPurchaseOrderProcessor
    {
        Task ProcessAsync(PurchaseOrderDto order);
    }
}