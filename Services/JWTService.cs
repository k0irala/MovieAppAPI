using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models.Api;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using MovieApplicationApi.Models.Entities;
using WebApplication1.Models.Entities;
namespace WebApplication1.Services
{
    public class JWTService(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        public LoginResponseModel Authenticate(LoginRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return new LoginResponseModel();
            var existingUser = dbContext.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (existingUser == null)
                return new LoginResponseModel();

            var token = GenerateAccessToken(existingUser);
            return token;
            
        }

        public LoginResponseModel GenerateAccessToken(User existingUser)
        {
            var issuer = configuration["JWTConfig:Issuer"];
            var audience = configuration["JWTConfig:Audience"];
            var key = configuration["JWTConfig:Key"];
            if(string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("JWT configuration values are not set properly.");
            }
            var tokenValidityMins = configuration.GetValue<int>("JWTConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(tokenValidityMins);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, existingUser.Username),
                    new Claim(JwtRegisteredClaimNames.Email,existingUser.Email),
                    new Claim("address",existingUser.Address),
                    new Claim("Role",existingUser.Roles == 1 ? "admin":"user")
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
                RefreshToken = GenerateRefreshToken(existingUser.Id),
                Username = existingUser.Username,
                AccessToken = accessToken,
                ExpiresIn = (int)(tokenExpiryTimeStamp - DateTime.Now).TotalSeconds
            };
        }

        //[HttpPost("Refresh")]
        //public IActionResult Refresh(RefreshToken token)
        //{
          
        //}
        public string GenerateRefreshToken(int userId)
        {
            var tokenId = Guid.NewGuid().ToString();
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            int expiryDays = 5;
            var expiryDate = DateTime.Now.AddDays(expiryDays);
             
            var token = new RefreshToken
            {
                Token = tokenId,
                RefreshUserToken = refreshToken,
                userId = userId
            };
            dbContext.RefreshTokens.Add(token);
            dbContext.SaveChanges();
            return refreshToken;
        }
        public RefreshToken GetRefreshToken(string refreshToken)
        {
            var validRefreshToken = dbContext.RefreshTokens
                .FirstOrDefault(rt => rt.RefreshUserToken == refreshToken);
            return validRefreshToken ?? new RefreshToken();
        }
        public void RevokeRefreshToken(string refreshToken)
        {
            var token = dbContext.RefreshTokens
                .FirstOrDefault(rt => rt.RefreshUserToken == refreshToken);
            if (token != null)
            {
                dbContext.RefreshTokens.Remove(token);
                dbContext.SaveChanges();
            }
        }
            
    }
}
