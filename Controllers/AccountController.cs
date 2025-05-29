using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApplicationApi.Models.Entities;
using WebApplication1.Data;
using WebApplication1.Models.Api;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(ApplicationDbContext dbContext, JWTService jwtService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<LoginResponseModel> Login(LoginRequestModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and Password is Required");
            }
            var accessToken = jwtService.Authenticate(model);

            if (accessToken == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return accessToken;
        }
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<LoginResponseModel>> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is required.");
            }
            var refreshToken1 = jwtService.GetRefreshToken(refreshToken);
            if (refreshToken1 == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            var users = await dbContext.Users.FirstOrDefaultAsync(x=>x.Id == refreshToken1.userId);
            if (users ==null) return Unauthorized("User not found.");

            var newAccessToken = jwtService.GenerateAccessToken(users);


            jwtService.RevokeRefreshToken(refreshToken1.RefreshUserToken);
            return Ok(new {AccessToken = newAccessToken });
        }
        [Authorize]
        [HttpPost("Logout")]
        public ActionResult Logout(string refreshToken)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (Request.Headers.ContainsKey("Authorization"))
            {
                Request.Headers["Authorization"] = string.Empty;
            }
            if(!string.IsNullOrEmpty(token))
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);   
                var expiryUTC = jwtToken.ValidTo;
                var expiryLocal = expiryUTC.ToLocalTime();
                dbContext.BlackListTokens.Add(new BlackListToken
                {
                    Token = token,
                    ExpiresAt = expiryLocal
                });
                dbContext.SaveChanges();
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is required.");
            }
            var refreshToken1 = jwtService.GetRefreshToken(refreshToken);
            if (refreshToken1 == null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            jwtService.RevokeRefreshToken(refreshToken1.RefreshUserToken);
            return Ok("Logged out successfully");

            //return Ok("Logged out successfully.");
        }
    }
}
