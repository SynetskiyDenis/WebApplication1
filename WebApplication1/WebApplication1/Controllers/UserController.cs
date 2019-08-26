using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.DbServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private  DBService _dbcon;

        public UserController( DBService dbcon)
        {
            _dbcon = dbcon;
        }

         /*
        // GET api/values 
        [HttpGet]
        public ActionResult<IEnumerable<UserInfo>> Get()
        {

        }
        */

        //авторизация пользователя
        // POST api/user/token        
        [HttpPost("token")]
        public IActionResult Login(UserLogin userLogin)
        {
            var jwtToken = _dbcon.Login(userLogin);

            if (jwtToken == null) return Unauthorized();

            return Ok(jwtToken);
        }

        //добавления нового пользователя
        // POST api/user
        [MyAuthorize]
        [HttpPost]
        public ActionResult<string> Post([FromBody] UserInfo userInfo)
        {
            if (_dbcon.FindUser(userInfo.username) != null)
            {
                return BadRequest("User " + userInfo.username + " already created");
            }          

            var resAddUser = _dbcon.AddUser(userInfo);

            if (resAddUser!=null) return BadRequest(resAddUser);
            
            return Ok("User " + userInfo.username + " was added successfully");
        }

        //изменение данных пользователя
        // PUT api/user
        [MyAuthorize]
        [HttpPut]
        public ActionResult<string> Put([FromBody] UserInfo userInfo)
        {
            var id = _dbcon.FindUser(User.Identity.Name);

            if (id == null)
            {
                return BadRequest("User " + userInfo.username + " not found");
            }

            var resUpdateUser = _dbcon.UpdateUser(Convert.ToInt32(id),userInfo);

            if (resUpdateUser != null) return BadRequest(resUpdateUser);

            return Ok("User " + userInfo.username + " was updated successfully");
        }

        //Logout пользователя указаного в теле запроса
        // DELETE api/user
        [MyAuthorize]        
        [HttpDelete("token")]
        [MyActionConstraint(true)]
        public ActionResult<string> Delete([FromBody] UserLogout userLogout)
        {
            if (_dbcon.FindUser(userLogout.username) == null)
            {
                return BadRequest("There is no such user");
            }

            var resLogoutUser = _dbcon.LogoutUser(userLogout.username);

            if (resLogoutUser != null) return BadRequest(resLogoutUser);

            return Ok("User " + userLogout.username + " logout successfully");
        }

        //Logout пользователя владельца Tokena
        // DELETE api/user
        [MyAuthorize]
        [HttpDelete("token")]
        [MyActionConstraint(false)]
        public ActionResult<string> Delete()
        {
            if (_dbcon.FindUser(User.Identity.Name) == null)
            {
                return BadRequest("There is no such user");
            }

            var resLogoutUser = _dbcon.LogoutUser(User.Identity.Name);

            if (resLogoutUser != null) return BadRequest(resLogoutUser);

            return Ok("User " + User.Identity.Name + " logout successfully");
        }

    }
}
