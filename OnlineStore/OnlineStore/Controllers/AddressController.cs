using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // api/address/add
        [HttpPost("add")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddAddress(AddressDTO addressDTO)
        {
            var result = await _addressService.AddAddressAsync(addressDTO);

            if (!result)
            {
                return BadRequest("Address added unsuccessfullly!"); 
            }

            return Ok(); 
        }
    }
}
