using OnlineStore.DBModels;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface IUserFrontendService
    {
        Task<bool> CreateUser(UserModel newUserModel);
        Task<bool> UpdateUserActiveMode(User user);
        Task<List<User>> GetUsers(); 
    }
}
