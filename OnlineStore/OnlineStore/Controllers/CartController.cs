using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        [HttpPost("add/{productId:int}")]
        public IActionResult AddToCart(int productId)
        {
            var userIdClaim = User.FindFirst("UserId"); 

            if(userIdClaim == null)
            {
                return BadRequest(); 
            }

            int userId = int.Parse(userIdClaim.Value);

            Console.WriteLine($"UserId: {userId}"); 
            Console.WriteLine($"ProductId: {productId}");

            return Ok(); 
        }
    }
}
