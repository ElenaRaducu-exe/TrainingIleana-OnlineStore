using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class UsersService : IUsersService
    {
        private readonly OnlineStoreContext _dbContext;

        public UsersService(OnlineStoreContext dbContext)
        {
            _dbContext = dbContext;
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
