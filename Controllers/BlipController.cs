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
    [Route("[controller]")]
    public class BlipController : ControllerBase
    {
        private readonly ILogger<BlipController> _logger;
        private readonly IBlipService blipService;
        private readonly IMongoService mongoService;
        private readonly ISendMessageBlip _sendMessageBlip;
        private readonly HttpClient _httpClient;


        public BlipController(ILogger<BlipController> logger, IBlipService blipService, IMongoService mongoService, ISendMessageBlip sendMessageBlip, HttpClient httpClient)
        {
            _logger = logger;
            this.blipService = blipService;
            this.mongoService = mongoService;
            _sendMessageBlip = sendMessageBlip;
            _httpClient = httpClient;
        }

        [HttpPost("testeRequest")]
        public async Task<IActionResult> TesteRequest()
        {
            var apiUrl = "https://allu-ai-messaging-api.digital.allugator.com/chat/message/receive";

            var payload = new
            {
                senderId = "sender",
                content = "request.Content",
                phoneNumber = "request.PhoneNumber",
                isAudio = false
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Adiciona o header Referer
            _httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://minha-app.azure.com");

            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

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
