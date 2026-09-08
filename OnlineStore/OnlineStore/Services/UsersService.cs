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
    }
}
