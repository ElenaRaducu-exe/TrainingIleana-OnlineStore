using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services;
using OnlineStore.Services.Backend.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoryController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoriesService.GetCategoriesAsync();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        // api/categories/get/name/{categoryId:int}
        [HttpGet("get/name/{categoryId:int}")]
        public async Task<IActionResult> GetCategoryName([FromRoute] int categoryId)
        {
            var result = await _categoriesService.GetCategoryNameByIdAsync(categoryId);

            if (result == null)
            {
                return NotFound();
            }

            //return new JsonResult(result);
            return Ok(result);
        }

        // api/categories/get/{categoryId:int}
        [HttpGet("get/{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
        {
            var result = await _categoriesService.GetCategoryByIdAsync(categoryId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
