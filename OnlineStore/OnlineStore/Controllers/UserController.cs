using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Backend;
using OnlineStore.Services.Backend.Contracts;

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

        // POST: api/users/add
        [HttpPost("add")]
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

        // GET: api/users/dashboard
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("dashboard/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(user);
        }

        // PUT: api/users/dashboard/update/active/{id:int}
        [HttpPut("dashboard/update/active/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserActiveMode([FromRoute] int userId)
        {
            var responseUpdatingStatusUser = await _userService.UpdateUserActiveMode(userId);

            if (responseUpdatingStatusUser == null)
            {
                return BadRequest("Change the active mode unsuccessful!");
            }

            return Ok();
        }
    }
}
