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

        [HttpGet("get/list")]
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

        // api/order/get/list/customer
        [HttpGet("get/list/customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrdersDTOCustomer()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return BadRequest("User not found!");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _orderService.GetOrdersDTOsCustomer(userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //api/get/{orderId:int}
        [HttpGet("get/{orderId:int}")]
        [Authorize(Roles = "Admin")]    
        public async Task<IActionResult> GetOrder([FromRoute]int orderId)
        {
            var result = await _orderService.GetOrderById(orderId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        // api/order/get/items
        [HttpGet("get/items/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderItemsDTOList([FromRoute]int orderId)
        {
            var result = await _orderService.GetOrderItemsDTOListById(orderId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        // api/order/get/details/{orderId:int}
        [HttpGet("get/details/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetailById([FromRoute]int orderId)
        {
            var result = await _orderService.GetOrderDetailsById(orderId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        // api/order/get/details-page/{orderId:int}
        [HttpGet("get/details-page/{orderId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetailsPage([FromRoute]int orderId)
        {
            var result = await _orderService.GetOrderDetailsPageDTO(orderId);

            if(result == null)
            {
                return NotFound(); 
            }

            return Ok(result); 
        }

        // api/order/update/status/{orderId:int}/{newStatusId:int}
        [HttpPut("update/status/{orderId:int}/{newStatusId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromRoute] int newStatusId)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return NotFound("User not found!");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _orderService.UpdateOrderStatus(orderId, newStatusId, userId);

            if(result == null)
            {
                return NotFound();
            }

            if(result == false)
            {
                return BadRequest();
            }

            return Ok();
        }

        // api/order/update/stock/cancelled-order/{orderId:int}
        [HttpPut("update/stock/cancelled-order/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductsStockAfterCancelledOrder([FromRoute] int orderId)
        {
            var result = await _orderService.UpdateProductsStockAfterCancelledOrder(orderId);

            if (result == false)
            {
                return NotFound();
            } 

            return Ok();
        }
    }
}
