using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface ICreateUserService
    {
        Task<bool> CreateUserAsync(CreateUserDTO newUser); 
    }
}
