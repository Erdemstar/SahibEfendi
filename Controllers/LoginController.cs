using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SahibEfendi.Model.UserModel;
using SahibEfendi.Service.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(User user)
        {

            var token = _userService.Authenticate(user);

            if (token is null)
            {
                return Unauthorized("There is problem Username or password");
            }
            return Ok(new { user.Email, token });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateUser(User user)
        {

            var files = _userService.CreateUser(user);
            if (files == false)
            {
                return BadRequest("There is an error while getting all user");
            }
            return Ok(user);

        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult PasswordForget(string email)
        {

            return Ok("İçeriği geliştirilecek" + email);

        }
    }
}
