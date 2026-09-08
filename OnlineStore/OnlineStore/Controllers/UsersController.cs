using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/users/dashboard")]
    public class UsersController : ControllerBase
    {
        private IUsersService _usersService;    

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _usersService.GetUsersAsync();

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
