using Microsoft.AspNetCore.Mvc;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Services.Interfaces;
using System.Text.Json;

namespace SkepsTicket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovideskController : ControllerBase
    {
        private readonly ILogger<MovideskController> _logger;
        private readonly IMovideskAPI _movideskApi;
        private readonly IMovideskService _movideskService;

        public MovideskController(ILogger<MovideskController> logger, IMovideskAPI movideskApi, IMovideskService movideskService)
        {
            _logger = logger;
            _movideskApi = movideskApi;
            _movideskService = movideskService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(dynamic ticket)
        {
            if (ticket.TryGetProperty("Id", out JsonElement idElement) && idElement.ValueKind == JsonValueKind.Number)
            {
                string ticketId = idElement.GetInt32().ToString();
                Console.WriteLine($"Webhook - Recebido - {ticketId}");

                _ = Task.Run(() => _movideskService.ProcessarNovoTicketReceptivo(ticketId));
            }

            return Ok();
        }


        [HttpPost("ticketAtivo")]
        public async Task<IActionResult> EnviarTicketAtivo([FromForm] EmailAtivoModel emailAtivo)
        {
            Console.WriteLine(JsonSerializer.Serialize(emailAtivo));
            var result = await _movideskService.CriarTicketAtivo(emailAtivo);

            if (string.IsNullOrWhiteSpace(result))
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("historico/{empresa}")]
        public async Task<IActionResult> EnviarTicketAtivo([FromRoute] string empresa, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string receiver)
        {
            Console.WriteLine(JsonSerializer.Serialize(emailAtivo));
            var result = await _movideskService.CriarTicketAtivo(emailAtivo);

            if (string.IsNullOrWhiteSpace(result))
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("ticket/{id}")]
        public async Task<IActionResult> GetTicketMovidesk([FromRoute] string id)
        {
            var result = await _movideskApi.GetTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", int.Parse(id));

            return Ok(result);
        }
    }
}
