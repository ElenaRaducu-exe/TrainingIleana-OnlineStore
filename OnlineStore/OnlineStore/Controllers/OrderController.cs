using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("get")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersDTO()
        {
            var result = await _orderService.GetOrdersDTO();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
