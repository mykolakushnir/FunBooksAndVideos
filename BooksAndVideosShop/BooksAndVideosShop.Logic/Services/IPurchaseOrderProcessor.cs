using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Domain.DTOs.Responses;
using BooksAndVideosShop.Logic.Results;

namespace BooksAndVideosShop.Logic.Services
{
    public interface IPurchaseOrderProcessor
    {
        Task<Result<PurchaseOrderResponseDto>> ProcessAsync(PurchaseOrderDto order);
    }
}