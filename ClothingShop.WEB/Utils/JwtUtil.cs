using ClothingShop.WEB.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClothingShop.WEB.Utils
{
    public class JwtUtil
    {
        public static string GetToken(IConfiguration configuration
            , Guid userId
            , string email
            , string role
            , int expirationDate)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                new[]
                {
                    new Claim(JwtClaimTypes.UserId, userId.ToString()),
                    new Claim(JwtClaimTypes.Email, email),
                    new Claim(JwtClaimTypes.Role, role)
                },
                expires: DateTime.Now.AddHours(expirationDate),
                signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
