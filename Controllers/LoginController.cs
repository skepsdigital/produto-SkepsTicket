using Microsoft.AspNetCore.Mvc;
using RestEase.Implementation;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Services.Interfaces;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SkepsTicket.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("code/generate")]
        public async Task<IActionResult> GenerateCode([FromBody]LoginModel loginModel)
        {
            var result = await _loginService.GenerateCode(loginModel.Email);
            
            if (result is null)
            {
                return Forbid();
            }

            return Ok(result);
        }


        [HttpPost("code/check")]
        public async Task<IActionResult> VerifyCode([FromBody] LoginModel loginModel)
        {
            var result = await _loginService.VerifyCode(loginModel.Email, loginModel.Code);

            if(result is null)
            {
                return Forbid();
            }

            return Ok(result);
        }

    }
}
