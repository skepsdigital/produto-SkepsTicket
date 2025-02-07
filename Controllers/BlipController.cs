using Microsoft.AspNetCore.Mvc;
using SkepsTicket.Model;
using SkepsTicket.Services.Interfaces;

namespace SkepsTicket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlipController : ControllerBase
    {
        private readonly ILogger<BlipController> _logger;
        private readonly IBlipService blipService;

        public BlipController(ILogger<BlipController> logger, IBlipService blipService)
        {
            _logger = logger;
            this.blipService = blipService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(BlipCloseTicketResponse blipCloseTicket)
        {
            Console.WriteLine($"Recebendo webhook Blip - {blipCloseTicket.BlipTicketId} - {blipCloseTicket.Identity} - {blipCloseTicket.Tags}");
            await blipService.ProcessarRespostaAtendente(blipCloseTicket);
            return Ok();
        }
    }
}
