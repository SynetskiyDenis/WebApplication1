using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace WebApplication1.Models
{
    //класс авторизации пользователей
    //при успешной авторизации возвращает JWT token
    public class LoginService
    {
        public List<UserInfo> users { get; set; }

        private readonly IConfiguration _configuration;

        public LoginService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Login(UserLogin userLogin)
        {
            var user = users.Where(x => x.username == userLogin.username && x.password == userLogin.password).SingleOrDefault();

            if (user == null)
            {
                return null;
            }
            user.isLogined = true;

            var signKey = Convert.FromBase64String(_configuration["Jwt:SignKey"]);
            var expiryDuration = int.Parse(_configuration["Jwt:ExpMinuts"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,              
                Audience = null,            
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expiryDuration),
                Subject = new ClaimsIdentity(new List<Claim> {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.username)
                    }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);            
            return token;
        }
    }
}
