using Microsoft.AspNetCore.Mvc;
using OnlineStore.JWTAuthentication.Models.DTOs;
using OnlineStore.JWTAuthentication.Services;
using OnlineStore.JWTAuthentication.Services.Contracts;

namespace OnlineStore.JWTAuthentication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var result = await _authService.LoginAsync(loginRequestDTO);

            if(result == null)
            {
                return Unauthorized("Invalid username or password!"); 
            }

            return Ok(result); 
        }
    }
}
