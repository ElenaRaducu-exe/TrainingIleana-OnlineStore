using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text; 

using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Services.Contracts;

namespace OnlineStore.JWTAuthentication.Services
{
    public class JWTService : IJWTService
    {
        private IConfiguration _configuration; 

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJWTToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.Username), 
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Customer" : "Admin")
            };

            var secretKey = _configuration["Jwt:Key"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1), 
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }
}
