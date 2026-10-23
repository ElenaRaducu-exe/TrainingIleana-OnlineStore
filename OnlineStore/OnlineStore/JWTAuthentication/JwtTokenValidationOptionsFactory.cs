using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OnlineStore.JWTAuthentication
{
    public static class JwtTokenValidationOptionsFactory
    {
        public static TokenValidationParameters CreateTokenValidationParameters(IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        }
    }
}
