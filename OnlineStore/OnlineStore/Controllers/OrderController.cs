using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.DTOs;
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

        [HttpPost("create")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequestDTO createOrderRequestDTO) 
        {
            var result = await _orderService.PlaceOrder(createOrderRequestDTO);

            if(result == false)
            {
                return BadRequest(); 
            }

            return Ok(); 
        }
    }
}
