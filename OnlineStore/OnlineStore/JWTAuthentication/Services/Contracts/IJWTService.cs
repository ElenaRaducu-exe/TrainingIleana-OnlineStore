using OnlineStore.DBModels; 

namespace OnlineStore.JWTAuthentication.Services.Contracts
{
    public interface IJWTService
    {
        string GenerateJWTToken(User user); 
    }
}
