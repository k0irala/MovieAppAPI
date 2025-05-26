using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models.Api;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace WebApplication1.Services
{
    public class JWTService(ApplicationDbContext dbContext, IConfiguration configuration) 
    {
        public async Task<LoginResponseModel> Authenticate(LoginRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return null;
            var existingUser = dbContext.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if(existingUser == null)
                return null;
            var issuer = configuration["JWTConfig:Issuer"];
            var audience = configuration["JWTConfig:Audience"];
            var key = configuration["JWTConfig:Key"];
            var tokenValidityMins = configuration.GetValue<int>("JWTConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, model.Username)
                }),
                Issuer = issuer,
                Audience = audience,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                Username = model.Username,
                AccessToken = accessToken,
                ExpiresIn = (int)(tokenExpiryTimeStamp - DateTime.UtcNow).TotalSeconds
            };  
        }
       
    }
}
