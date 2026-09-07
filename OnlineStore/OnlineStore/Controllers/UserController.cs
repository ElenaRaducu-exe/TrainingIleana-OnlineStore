using Microsoft.AspNetCore.Mvc;

using OnlineStore.Services.Contracts; 
using OnlineStore.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private ICreateUserService _createUserService;

        public UserController(ICreateUserService createUserService)
        {
            _createUserService = createUserService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDTO newUser)
        {
            var result = await _createUserService.CreateUserAsync(newUser);

            if (!result)
            {
                return BadRequest("Username already exists!"); 
            }

            return Ok("User created successfully!"); 
        }
    }
}
