using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using TestMakerFreeWebApp.ViewModels;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using System.Security.Claims;

using System.Text;

namespace TestMakerFreeWebApp.Controllers
{
    public class TokenController : BaseApiController
    {

        #region Constructor
        public TokenController(ApplicationDbContext context,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager,
                                IConfiguration configuration) : base(context, roleManager, userManager, configuration) { }

        #endregion

        [HttpPost("Auth")]
        public async Task<IActionResult> Jwt([FromBody] TokenRequestViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            switch(model.grant_type){
                case "password":
                    return await GetToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    return new UnauthorizedResult();
            }
        }

        public async Task<IActionResult> GetToken(TokenRequestViewModel model) {
            try {

                var user = await UserManager.FindByNameAsync(model.username);

                //fallback to support email address instead
                if (user == null && model.username.Contains("@")) {
                    user = await UserManager.FindByEmailAsync(model.username);
                }

                // if user doesnt exist or password mismatch
                if (user == null || !await UserManager.CheckPasswordAsync(user, model.password)) {
                    return new UnauthorizedResult();
                }

                //user cleared

                /* removing to use new functions 

                DateTime now = DateTime.UtcNow;

                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
                };

                var tokenExpirationMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes"); 
                var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

                var token = new JwtSecurityToken(
                            issuer: Configuration["Auth:Jwt:Issuer"],
                            audience: Configuration["Auth:Jwt:Audience"],
                            claims: claims,
                            notBefore: now,
                            expires:now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                            signingCredentials: new SigningCredentials(issuerSigningKey,SecurityAlgorithms.HmacSha256));

                var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

                // build & return the response
                var response = new TokenResponseViewModel()
                {
                    token = encodedToken,
                    expiration = tokenExpirationMins
                };

                return Json(response);*/

                // username & password matches: create the refresh token
                var rt = CreateRefreshToken(model.client_id, user.Id);

                // add the new refresh token
                DbContext.Tokens.Add(rt);

                // persist changes in the DB
                DbContext.SaveChanges();

                var access = CreateAccessToken(user.Id, rt.Value);

                return Json(access);

            }
            catch (Exception ex)
            {
                return new UnauthorizedResult();
            }
        }

        public async Task<IActionResult> RefreshToken(TokenRequestViewModel model)
        {
            try
            {
                var rt = DbContext.Tokens.FirstOrDefault(t => t.ClientId == model.client_id && t.Value == model.refresh_token);

                if (rt == null) {return new UnauthorizedResult();}

                var user = await UserManager.FindByIdAsync(rt.UserId);

                if (user == null) { return new UnauthorizedResult(); }

                // generate a new refresh token
                var rtNew = CreateRefreshToken(model.client_id, rt.UserId);

                // invalidate the old refresh token (by deleting it)
                DbContext.Tokens.Remove(rt);
                // add the new refresh token
                DbContext.Tokens.Add(rtNew);

                // persist changes in the DB
                DbContext.SaveChanges();

                // create a new access token...
                var response = CreateAccessToken(rtNew.UserId, rtNew.Value);

                return Json(response);
            }
            catch (Exception e) {
                return new UnauthorizedResult();
            }
        }

        private Token CreateRefreshToken(string clientId, string userId) {
            return new Token()
            {
                ClientId = clientId,
                UserId = userId,
                Type = 0,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow
            };
        }

        private TokenResponseViewModel CreateAccessToken(string userId,string refreshToken) {

            DateTime now = DateTime.UtcNow;

            // add the registered claims for JWT (RFC7519).
            // For more info, see https://tools.ietf.org/html/rfc7519#section-

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
        // TODO: add additional claims here
            };

            var tokenExpirationMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

            var token = new JwtSecurityToken(
                    issuer: Configuration["Auth:Jwt:Issuer"],
                    audience:Configuration["Auth:Jwt:Audience"],
                    claims:claims,
                    notBefore:now,
                    expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                    signingCredentials : new SigningCredentials(issuerSigningKey,SecurityAlgorithms.HmacSha256)
                );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseViewModel()
            {
                token = encodedToken,
                expiration = tokenExpirationMins,
                refresh_token = refreshToken
            };
        }
    }
}