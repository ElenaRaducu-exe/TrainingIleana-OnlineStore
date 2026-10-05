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
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return BadRequest("User not found!");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _addressService.AddAddressAsync(addressDTO, userId);

            if (!result)
            {
                return BadRequest("Address added unsuccessfullly!"); 
            }

            return Ok(); 
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressByUserId()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return BadRequest("User not found!");
            }

            int userId = int.Parse(userIdClaim.Value);

            var addressDTO = await _addressService.GetAddressesByUserId(userId);

            if(addressDTO == null)
            {
                return BadRequest("Addresses not found!");
            }

            return Ok(addressDTO);
        }
    }
}
