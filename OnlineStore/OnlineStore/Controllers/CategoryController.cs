using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private ICategoriesService _categoriesService;

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
    }
}
