using Microsoft.AspNetCore.Mvc;
using RestEase.Implementation;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Services.Interfaces;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SkepsTicket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IMongoService _mongoService;
        public CompanyController(ILoginService loginService, IMongoService mongoService = null)
        {
            _loginService = loginService;
            _mongoService = mongoService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CriarEmpresa([FromBody]EmpresasInfoMongo empresa)
        {   
            await _mongoService.InserirEmpresasInfo(empresa);
            return Ok();
        }

        [HttpPost("search/create")]
        public async Task<IActionResult> SalvarPesquisa([FromBody] SearchMongo searchMongo)
        {
            await _mongoService.InserirSearch(searchMongo);
            return Ok();
        }
    }
}
