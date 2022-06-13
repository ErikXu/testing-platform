using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login()
        {
            var result = new LoginResult
            {
                Code = 20000,
                Data = new LoginData
                {
                    Token = "admin-token"
                }
            };

            return Ok(result);
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            var info = new UserInfo
            {
                Code = 20000,
                Data = new UserInfoData
                {
                    Roles = new List<string> { "admin" },
                    Introduction = "I am a super administrator",
                    Avatar = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif",
                    Name = "Admin"
                }
            };

            return Ok(info);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var result = new LogoutResult
            {
                Code = 20000,
                Data = "success"
            };

            return Ok(result);
        }
    }


    public class LoginResult
    {
        public int Code { get; set; }

        public LoginData Data { get; set; }
    }

    public class LoginData
    {
        public string Token { get; set; }
    }


    public class UserInfo
    {
        public int Code { get; set; }

        public UserInfoData Data { get; set; }
    }

    public class UserInfoData
    {
        public List<string> Roles { get; set; }

        public string Introduction { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }
    }


    public class LogoutResult
    {
        public int Code { get; set; }

        public string Data { get; set; }
    }
}
