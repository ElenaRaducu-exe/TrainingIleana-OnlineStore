using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;

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

    }
}
