using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SahibEfendi.Handler;
using SahibEfendi.Model.UserModel;
using SahibEfendi.Service.UserService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserById(string id)
        {
            var users = _userService.GetUserById(id);
            return Ok(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUser()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetUser()
        {

            var userID =  UserHandler.FindUserIDFromJWTToken(Request.Headers["Authorization"].ToString().Split(" ")[1]);

            var user = _userService.GetUserById(userID); 
            if (user is null)
            {
                return BadRequest("There is a problem while find a current user");
            }

            return Ok(user);
        }

        

    }
}
