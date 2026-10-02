using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using OnlineStore.Models.DTOs;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _userService;

        public UserController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDTO newUser)
        {
            var result = await _userService.CreateUserAsync(newUser);

            if (!result)
            {
                return BadRequest("Username already exists!"); 
            }

            return Ok("User created successfully!"); 
        }

        //[Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuthentication()
        {
            return Ok("You are authenticated!"); 
        }
    }
}
