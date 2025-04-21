using BooksAndVideosShop.Domain.DTOs;
using BooksAndVideosShop.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BooksAndVideosShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderProcessor _processor;

        public PurchaseOrderController(IPurchaseOrderProcessor processor)
        {
            _processor = processor;
        }        

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] PurchaseOrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Invalid order.");
            }

            await _processor.ProcessAsync(orderDto);
            
            return Ok(new 
            {
                message = "Purchase order processed successfully."
            });
        }
    }
}
