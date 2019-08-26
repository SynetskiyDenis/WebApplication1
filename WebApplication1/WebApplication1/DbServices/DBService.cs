using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication1.DbServices
{
    public class DBService
    {
        private readonly IConfiguration _configuration;

        public DBService (IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        public object FindUser(string username)
        {
            SqlConnection _connection = new SqlConnection(_configuration["DB:ConnectionString"]);

            _connection.Open();

            var txt = String.Format("Select id from Users where username='{0}'", username);

            SqlCommand cmd = new SqlCommand(txt, _connection);

            var res = cmd.ExecuteScalar();

            _connection.Close();

            return res;
        }

        public object AddUser(UserInfo userInfo)
        {
            SqlConnection _connection = new SqlConnection(_configuration["DB:ConnectionString"]);

            _connection.Open();

            var txt = String.Format("Insert into Users (username, password, email, islogined) VALUES ('{0}', '{1}', '{2}', 0)", userInfo.username, userInfo.password,userInfo.email);

            SqlCommand cmd = new SqlCommand(txt, _connection);            

            try
            {
                cmd.ExecuteNonQuery();

                _connection.Close();

                return null;
            }
            catch (SqlException ex)
            {
                _connection.Close();

                return ex.Message;
            }
        }

        public object UpdateUser(int id, UserInfo userInfo)
        {
            SqlConnection _connection = new SqlConnection(_configuration["DB:ConnectionString"]);

            _connection.Open();

            var txt = String.Format("Update Users set username = '{0}', password = '{1}', email = '{2}' where id={3}", userInfo.username, userInfo.password, userInfo.email,id);

            SqlCommand cmd = new SqlCommand(txt, _connection);

            try
            {
                cmd.ExecuteNonQuery();

                _connection.Close();

                return null;
            }
            catch (SqlException ex)
            {
                _connection.Close();

                return ex.Message;
            }

        }

        public object LogoutUser(string username)
        {
            SqlConnection _connection = new SqlConnection(_configuration["DB:ConnectionString"]);

            _connection.Open();

            var txt = String.Format("Update Users set islogined=0 where username='{0}'", username);

            SqlCommand cmd = new SqlCommand(txt, _connection);

            try
            {
                cmd.ExecuteNonQuery();

                _connection.Close();

                return null;
            }
            catch (SqlException ex)
            {
                _connection.Close();

                return ex.Message;
            }

        }

        public string Login(UserLogin userLogin)
        {
            SqlConnection _connection = new SqlConnection(_configuration["DB:ConnectionString"]);

            _connection.Open();

            var txt = String.Format("Select id from Users where username='{0}' and password='{1}'", userLogin.username, userLogin.password);

            SqlCommand cmd = new SqlCommand(txt,_connection);

            var id = cmd.ExecuteScalar();

            if (id == null) return null;

            txt = String.Format("Update Users SET islogined=1 WHERE id={0}", id);

            cmd.CommandText = txt;

            cmd.ExecuteNonQuery();

            _connection.Close();

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
                        new Claim(ClaimsIdentity.DefaultNameClaimType, userLogin.username)
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
