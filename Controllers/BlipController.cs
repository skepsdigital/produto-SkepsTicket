using Microsoft.AspNetCore.Mvc;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Services.Interfaces;
using System.Net.Sockets;
using System.Text.Json;

namespace SkepsTicket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlipController : ControllerBase
    {
        private readonly ILogger<BlipController> _logger;
        private readonly IBlipService blipService;
        private readonly IMongoService mongoService;
        private readonly ISendMessageBlip _sendMessageBlip;


        public BlipController(ILogger<BlipController> logger, IBlipService blipService, IMongoService mongoService, ISendMessageBlip sendMessageBlip)
        {
            _logger = logger;
            this.blipService = blipService;
            this.mongoService = mongoService;
            _sendMessageBlip = sendMessageBlip;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(BlipCloseTicketResponse blipCloseTicket)
        {
            Console.WriteLine($"Recebendo webhook Blip - {blipCloseTicket.BlipTicketId} - {blipCloseTicket.Identity} - {blipCloseTicket.Tags}");
            await blipService.ProcessarRespostaAtendente(blipCloseTicket);
            return Ok();
        }

        [HttpPost("Reativar")]
        public async Task<IActionResult> Reativar(string stringsGrande)
        {
            var strings = stringsGrande.Split(",");

            foreach (var ident in strings)
            {
                string requestEnviarMsgJson = JsonSerializer.Serialize(new { email = ident.Replace("@", "%40"), mensagem = "Reativando", identificadorBot = "skepstickethml", contrato = "brasgaming" });
                var enviarResponse = await _sendMessageBlip.EnviarMensagem(requestEnviarMsgJson);

                if(enviarResponse.Contains("false"))
                {
                    await Task.Delay(10000);
                }
            }

            return Ok();
        }

        [HttpGet("fila")]
        public async Task<IActionResult> Fila()
        {
            _ = Task.Run(async () =>
            {
                Console.WriteLine("Iniciando processamento de fila");
                var item = await mongoService.BuscarItemNaFila();

                while (item is not null)
                {
                    var criarResponse = await _sendMessageBlip.CriarAccount(item.CriarContaJson);
                    if (criarResponse.Contains("true"))
                    {
                        var enviarResponse = await _sendMessageBlip.EnviarMensagem(item.EnviarMensagem);

                        if (enviarResponse.Contains("false"))
                        {
                            Console.WriteLine($"Problemas ao enviar email para {item.EnviarMensagem}");
                            //await mongoService.InserirComandoNaFila(item);
                            await Task.Delay(3000);
                        }
                    }
                    else
                    {
                        item.Id = new MongoDB.Bson.ObjectId();
                        await mongoService.InserirComandoNaFila(item);
                    }

                    item = await mongoService.BuscarItemNaFila();
                }
            });

            return Ok();
        }
    }
}
