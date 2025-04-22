using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BooksAndVideosShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IPurchaseOrderProcessor _processor;

        public PurchaseOrderController(IPurchaseOrderProcessor processor, ILogger<PurchaseOrderController> logger)
        {
            _logger = logger;
            _processor = processor;
        }        

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] PurchaseOrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Invalid order.");
            }

            var orderResult = await _processor.ProcessAsync(orderDto);

            if (!orderResult.IsSuccess)
            {
                _logger.LogWarning("Failed to process order: {Error}", orderResult.Error);
                return BadRequest(new
                {
                    error = orderResult.Error
                });
            }

            _logger.LogInformation("Processed order successfully: {OrderId}", orderResult.Data!.Id);
            return Ok(new 
            {
                message = "Purchase order processed successfully.",
                order = orderResult.Data
            });
        }
    }
}
