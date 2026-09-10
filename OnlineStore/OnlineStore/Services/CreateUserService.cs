using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;
using OnlineStore.Models;

namespace OnlineStore.Services
{
    public class CreateUserService : ICreateUserService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher; 

        public CreateUserService(OnlineStoreContext onlineStoreContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = onlineStoreContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> CreateUserAsync(CreateUserDTO newUser)
        {
            var existingUser = await _dbContext.Users.AnyAsync(user =>
                user.Username == newUser.Username);

            if (existingUser)
            {
                return false; 
            }

            User currentUser = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                RoleId = (int)newUser.Role,
                IsActive = true
            };

            currentUser.PasswordHash = _passwordHasher.HashPassword(currentUser, newUser.Password);

            _dbContext.Users.Add(currentUser);

            await _dbContext.SaveChangesAsync();

            return true; 
        }
    }
}
