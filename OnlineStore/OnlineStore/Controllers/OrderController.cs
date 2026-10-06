using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // api/order/create/{addressId}/{userId}/{cartId}
        [HttpPost("create/{addressId}/{userId}/{cartId}")]
        public async Task<IActionResult> PlaceOrder(int addressId, int userId, int cartId)
        {
            var result = await _orderService.PlaceOrder(addressId, userId, cartId);

            if(result == false)
            {
                return BadRequest(); 
            }

            return Ok(); 
        }
    }
}
