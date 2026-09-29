using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

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
        [Authorize(Roles = "Admin")]
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
            var result = await _productService.GetProductDTOAsyncById(id);

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

        [HttpGet("pagination")]
        public async Task<IActionResult> GetProductDTOsPagination([FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _productService.GetProductDTOListPagination(pageNumber, pageSize);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredSortedProductDTOs([FromQuery] ProductFiltersModel productFilters)
        {
            if(productFilters.PageNumber < 1 || productFilters.PageSize < 1)
            {
                return BadRequest("Invalid pagination parameters");
            }

            var result = await _productService.GetFilteredSortedProductDTOs(productFilters);

            if(result == null)
            {
                return BadRequest("Not found!");
            }

            return Ok(result);
        }

        [HttpGet("filtered/count")]
        public async Task<IActionResult> GetFilteredSortedProductsCount([FromQuery] ProductFiltersModel productFilters)
        {
            var result = await _productService.GetFilteredSortedProductsCount(productFilters);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetProductsCount()
        {
            var result = await _productService.GetProductsCount();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpDelete("delete/product/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id); 

            if(result == false)
            {
                return BadRequest();
            }

            return Ok(); 
        }

        [HttpPut("update/product/active-status/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductActiveMode([FromRoute] int id)
        {
            var updatedProduct = await _productService.ChangeActiveMode(id); 

            if (updatedProduct == null)
            {
                return BadRequest("Product not found!");
            }

            return Ok(updatedProduct);
        }

        //"api/admin/products/update/product/{id:int}"
        [HttpPut("update/product/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductDTO productDetails)
        {
            var productDTO = await _productService.GetProductDTOAsyncById(id); 

            if(productDTO == null)
            {
                return BadRequest("Product not found!");
            }

            await _productService.UpdateProduct(productDetails);

            return Ok();
        }

        // api/admin/products/{productId:int}
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductDTOById([FromRoute] int productId)
        {
            var productDTO = await _productService.GetProductDTOById(productId);

            if(productDTO == null)
            {
                return BadRequest();
            }

            return Ok(productDTO);
        }
    }
}
