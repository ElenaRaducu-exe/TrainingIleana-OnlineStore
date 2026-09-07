using OnlineStore.JWTAuthentication.Models.DTOs;

namespace OnlineStore.JWTAuthentication.Services.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest); 
    }
}
