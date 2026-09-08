using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class UsersService : IUsersService
    {
        private OnlineStoreContext _dbConext;

        public UsersService(OnlineStoreContext dbConext)
        {
            _dbConext = dbConext;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _dbConext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbConext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> UpdateUserActiveMode(int id)
        {
            var user = await GetUserByIdAsync(id); 

            if (user == null)
            {
                return null;
            }

            user.IsActive = !user.IsActive;

            _dbConext.SaveChanges();

            return user;
        }
    }
}
