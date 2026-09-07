using OnlineStore.Models;

namespace OnlineStore.Services.Contracts
{
    public interface ICreateUserService
    {
        Task<bool> CreateUserAsync(CreateUserDTO newUser); 
    }
}
