using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinoDbAPI.Settings;
using SinoDbAPI.Services;

namespace SinoDbAPI.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthenticationSettings _settings;

        public JwtMiddleware(RequestDelegate next, IAuthenticationSettings settings)
        {
            _next = next;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context, IUsersService usersService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                attachUserToContext(context, usersService, token);
            }
            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IUsersService usersService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_settings.JwtSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                context.Items["User"] = usersService.GetById(userId);
            }
            catch
            {

            }
        }
    }
}