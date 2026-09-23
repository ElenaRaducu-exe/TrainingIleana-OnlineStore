using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartProductsService _cartProductsService;
        private readonly IProductService _productService;

        public CartController(ICartProductsService cartProductsService,
                              IProductService productService)
        {
            _cartProductsService = cartProductsService;
            _productService = productService;
        }

        [HttpGet("items/user")]
        public async Task<IActionResult> GetCartItems()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _cartProductsService.GetCartItemsByUser(userId);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("add/products-list/{productId:int}")]
        public async Task<IActionResult> AddToCartProductsList(int productId)
        {
            var productDTO = await _productService.GetProductDTOAsync(productId);

            if(productDTO == null)
            {
                return BadRequest("Product not found!");
            }

            await _cartProductsService.AddProductToCartProductsList(productDTO);

            return Ok(); 
        }

        [HttpPost("add/product/{productId:int}")]
        public async Task<IActionResult> AddProductToCart(int productId)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            int userId = int.Parse(userIdClaim.Value);

            Console.WriteLine($"UserId: {userId}");

            await _cartProductsService.AddProductToCart(productId, userId);

            return Ok();
        }

        // api/cart/update/quantity/{cartItemId:int}
        [HttpPut("update/quantity/{cartItemId:int}")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromRoute]int cartItemId, [FromBody]int newQuantity)
        {
            var result = await _cartProductsService.UpdateCartItemQuantity(cartItemId, newQuantity);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        // api/cart/delete/{cartItemId:int}
        [HttpDelete("delete/{cartItemId:int}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute]int cartItemId)
        {
            var result = await _cartProductsService.DeleteCartItem(cartItemId);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
