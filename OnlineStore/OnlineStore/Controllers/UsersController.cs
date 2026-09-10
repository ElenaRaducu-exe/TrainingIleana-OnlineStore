using Microsoft.AspNetCore.Mvc;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/users/dashboard")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;    

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

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserBtId([FromRoute] int id)
        {
            var user = await _usersService.GetUserByIdAsync(id); 

            if(user == null)
            {
                return BadRequest();
            }

            return Ok(user); 
        }

        [HttpPut]
        [Route("update/active/{id:int}")]
        public async Task<IActionResult> UpdateUserActiveMode([FromRoute] int id)
        {
            var user = await _usersService.GetUserByIdAsync(id);

            if(user == null)
            {
                return BadRequest("User not found!");
            }
            else { 
                bool isActiveInitial = user.IsActive; 
                var userUpdated = await _usersService.UpdateUserActiveMode(user);
            
                if (isActiveInitial == userUpdated.IsActive)
                {
                    return BadRequest("Change the active mode unsuccessful!");
                }

                return Ok(userUpdated);
            }
        }
    }
}
