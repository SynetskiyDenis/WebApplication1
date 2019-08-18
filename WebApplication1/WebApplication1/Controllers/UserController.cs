using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //создаем список Пользователей с добавлением Пользователя superadmin
        private static List<UserInfo> userInfoList =
            new List<UserInfo> {
                    new UserInfo {
                    username = "superadmin",
                    password = "pswd",
                    email = "mail@gmail.com",
                    isLogined = false
                }
            };
           
        private readonly LoginService _loginService; 

        public UserController(LoginService loginService)
        {
            _loginService = loginService;
            _loginService.users = userInfoList;
        }

        //функция проверки "LOGOUT" пользователя
        private int CheckLogin()
        {
            var id = userInfoList.FindIndex(u => u.username == User.Identity.Name);

            if (id< 0) return -2;

            if (!userInfoList.ElementAt(id).isLogined) return -1;

            return id;
        }
        
        /*
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<UserInfo>> Get()
        {
            return userInfoList;
        }
        */
    
        //авторизация пользователя
        // POST api/user/token        
        [HttpPost("token")]
        public IActionResult Login(UserLogin userLogin)
        {
            var jwtToken = _loginService.Login(userLogin);

            if (jwtToken == null) return Unauthorized();

            return Ok(jwtToken);
        }

        //добавления нового пользователя
        // POST api/user
        [Authorize]
        [HttpPost]
        public ActionResult<string> Post([FromBody] UserInfo userInfo)
        {
            var id = CheckLogin();
            switch (id)
            {
                case -2: return BadRequest("There is no such users");
                case -1: return Unauthorized();
            }

            if (userInfoList.Any(u => u.username == userInfo.username))
            {
                return BadRequest("User " + userInfo.username + " already created"); 
            }

            userInfoList.Add(userInfo);

            return Ok("User " + userInfo.username + " was added successfully");
        }

        //изменение данных пользователя
        // PUT api/user
        [Authorize]
        [HttpPut]
        public ActionResult<string> Put([FromBody] UserInfo userInfo)
        {
            var id = CheckLogin();
            switch (id)
            {
                case -2: return BadRequest("There is no such users");
                case -1: return Unauthorized();
            }

            userInfoList.ElementAt(id).username = userInfo.username;
            userInfoList.ElementAt(id).password = userInfo.password;
            userInfoList.ElementAt(id).email = userInfo.email;

            return Ok("User " + userInfo.username + " was updated successfully");
        }

        //Logout пользователя указаного в теле запроса
        // DELETE api/user
        [Authorize]
        [HttpDelete("token")]
        [MyActionConstraint(true)]
        public ActionResult<string> Delete([FromBody] UserLogout userLogout)
        {
            var id = CheckLogin();
            switch (id)
            {
                case -2: return BadRequest("There is no such users");
                case -1: return Unauthorized();
            }

            id = userInfoList.FindIndex(u => u.username == userLogout.username);

            if (id < 0) return BadRequest("There is no such users");
            
            userInfoList.ElementAt(id).isLogined = false;

            return Ok("User " + userLogout.username + " logout successfully");
        }

        //Logout пользователя владельца Tokena
        // DELETE api/user
        [Authorize]
        [HttpDelete("token")]
        [MyActionConstraint(false)]
        public ActionResult<string> Delete()
        {
            var id = CheckLogin();
            switch (id)
            {
                case -2: return BadRequest("There is no such users");
                case -1: return Unauthorized();
            }

            userInfoList.ElementAt(id).isLogined = false;

            return Ok("User " + User.Identity.Name + " logout successfully");
        }

    }
}
