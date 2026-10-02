using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Models;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;
using System.Security.Claims;

namespace OnlineStore.Services
{
    public class UsersService : IUsersService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly JWTAuthenticationStateProvider _jwtStateProvider;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper; 

        public UsersService(OnlineStoreContext dbContext, 
                            JWTAuthenticationStateProvider jwtAuthenticationStateProvider,
                            IPasswordHasher<User> passwordHasher, 
                            IMapper mapper)
        {
            _dbContext = dbContext;
            _jwtStateProvider = jwtAuthenticationStateProvider;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> UpdateUserActiveMode(User user)
        {
            if (user == null)
            {
                return null;
            }

            user.IsActive = !user.IsActive;

            _dbContext.SaveChanges();

            return user;
        }

        public async Task<bool> CreateUserAsync(UserDTO newUser)
        {
            //Console.WriteLine((int)newUser.Role);
            newUser.IsActive = true;

            var existingUser = await _dbContext.Users.AnyAsync(user =>
                user.Username == newUser.Username);

            if (existingUser)
            {
                return false;
            }

            User currentUser = _mapper.Map<User>(newUser);
            currentUser.PasswordHash = _passwordHasher.HashPassword(currentUser, newUser.Password);

            _dbContext.Users.Add(currentUser);


            if (newUser.Role == null)
            {
                currentUser.RoleId = (int)UserRoleEnum.Customer;
            }
            
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
