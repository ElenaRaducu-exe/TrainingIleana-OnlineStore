using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.JWTAuthentication.Models.DTOs;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.DBModels;

namespace OnlineStore.JWTAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJWTService _jwtService; 

        public AuthService(OnlineStoreContext onlineStoreContext, 
                           IPasswordHasher<User> passwordHasher,
                           IJWTService jwtService)
        {
            _dbContext = onlineStoreContext;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == loginRequest.Username);

            if(currentUser == null || !currentUser.IsActive)
            {
                return null;
            }

            // password verification 
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword
                    (currentUser, currentUser.PasswordHash, loginRequest.Password);

            if(passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // JWT Token generating 
            var token = _jwtService.GenerateJWTToken(currentUser);

            return new LoginResponseDTO
            {
                TokenJWT = token,
                Username = loginRequest.Username,
                Role = currentUser.RoleId == 1 ? "Customer" : "Admin"
            };  
        }
    }
}
