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

        public UsersService(OnlineStoreContext dbContext, JWTAuthenticationStateProvider jwtAuthenticationStateProvider)
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

        public async Task<User?> GetUser()
        {
            var authStateUser = await _jwtStateProvider.GetAuthenticationStateAsync();

            if (authStateUser == null)
            {
                return null;
            }
            var user = authStateUser.User;
             
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var claim = user.FindFirst("UserId");

                if(claim != null && int.TryParse(claim.Value, out var parsedId))
                {
                    return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == parsedId);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null; 
            }
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
