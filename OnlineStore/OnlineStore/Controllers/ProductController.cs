using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("admin/products")]
    public class ProductController : ControllerBase
    {
        private IProductService _productService; 

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("add/product")]
        public async Task<IActionResult> AddProduct(ProductDTO newProduct)
        {
            var result = await _productService.AddProductAsync(newProduct);

            if (!result)
            {
                return BadRequest("Product added unsuccessfully!");
            }

            return Ok("Product added successfully!");
        }
    }
}
