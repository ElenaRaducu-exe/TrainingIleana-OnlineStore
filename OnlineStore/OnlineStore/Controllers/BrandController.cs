using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services;
using OnlineStore.Services.Backend.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandsService _brandsService;

        public BrandController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _brandsService.GetBrandsListAsync();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        // api/brands/get/name/{brandId:int}
        [HttpGet("get/name/{brandId:int}")]
        public async Task<IActionResult> GetBrandName([FromRoute] int brandId)
        {
            var result = await _brandsService.GetBrandNameByIdAsync(brandId);

            if (result == null)
            {
                return NotFound();
            }

            //return new JsonResult(result);
            return Ok(result);
        }

        // api/brands/get/{brandId:int}
        [HttpGet("get/{brandId:int}")]
        public async Task<IActionResult> GetBrandById([FromRoute] int brandId)
        {
            var result = await _brandsService.GetBrandByIdAsync(brandId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
