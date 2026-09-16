using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.Services.Contracts;
using System.Security.Claims;

namespace OnlineStore.Services
{
    public class UsersService : IUsersService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly JWTAuthenticationStateProvider _jwtStateProvider;

        public UsersService(OnlineStoreContext dbContext, 
                            JWTAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _dbContext = dbContext;
            _jwtStateProvider = jwtAuthenticationStateProvider;
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
    }
}
