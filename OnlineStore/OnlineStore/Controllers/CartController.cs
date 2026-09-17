using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartProductsService _cartProductsService;

        public CartController(ICartProductsService cartProductsService)
        {
            _cartProductsService = cartProductsService;
        }

        [HttpPost("add/products-list/{productId:int}")]
        public async Task<IActionResult> AddToCartProductsList(int productId)
        {
            var userIdClaim = User.FindFirst("UserId"); 

            if(userIdClaim == null)
            {
                return BadRequest(); 
            }

            int userId = int.Parse(userIdClaim.Value);

            await _cartProductsService.AddProductToCart(productId, userId);

            Console.WriteLine($"UserId: {userId}"); 
            Console.WriteLine($"ProductId: {productId}");

            return Ok(); 
        }
    }
}
