using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService; 

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

        [HttpGet("get/product/{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productService.GetProductDTOAsync(id);

            if (result == null)
            {
                return BadRequest($"Product {id} not found!");
            }

            return Ok(result); 
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsDTO()
        {
            var result = await _productService.GetProductDTOListAsync();

            if(result == null) {
                return BadRequest(); 
            }

            return Ok(result); 
        }

        [HttpDelete("delete/product/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id); 

            if(result == false)
            {
                return BadRequest();
            }

            return Ok(); 
        }
    }
}
