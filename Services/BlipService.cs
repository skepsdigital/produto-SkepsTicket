using Newtonsoft.Json;
using RestEase;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Services.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkepsTicket.Services
{
    public class BlipService : IBlipService
    {
        private readonly IMovideskAPI _movideskApi;
        private readonly IMongoService _mongoService;

        public BlipService(IMovideskAPI movideskApi, IMongoService mongoService)
        {
            _movideskApi = movideskApi;
            _mongoService = mongoService;
        }


        public async Task ProcessarRespostaAtendente(BlipCloseTicketResponse blipCloseTicket)
        {
            try
            {
                var blipSender = RestClient.For<IBlipSender>($"https://{blipCloseTicket.Contrato}.http.msging.net");

                var obterHistorico = new
                {
                    id = Guid.NewGuid().ToString(),
                    method = "get",
                    uri = $"/threads/{blipCloseTicket.Identity.Replace("%40", "%2540")}?refreshExpiredMedia=true"
                };

                string requestContatoJson = JsonSerializer.Serialize(new BlipCommandModel { Id = Guid.NewGuid().ToString(), Method = "get", To = "postmaster@crm.msging.net", Uri = $"/contacts/{blipCloseTicket.Identity.Replace("%40", "%2540")}" });
                var contatoBlip = await blipSender.SendCommandAsync(requestContatoJson, blipCloseTicket.BotKey);

                string obterHistoricoJson = JsonSerializer.Serialize(obterHistorico);
                var historicoDynamic = await blipSender.SendCommandAsync(obterHistoricoJson, blipCloseTicket.BotKey);

                string jsonString = JsonConvert.SerializeObject(historicoDynamic);

                var historico = JsonConvert.DeserializeObject<BlipHistorico>(jsonString);

                var result = historico.Resource.Items.TakeWhile(item => item.Direction != "received").Reverse().ToList();

                var respostaAtendente = string.Join(Environment.NewLine, result.Where(r => r.Type.Equals("text/plain")).Select(r => (string)r.Content));

                if (blipCloseTicket.Tags.Contains("Resolvido"))
                {
                    //var cliente = await _mongoService.GetByIdAsync(emailCliente);

                    Console.WriteLine($"{contatoBlip.resource.extras.ticketIdMovidesk} - Ticket resolvido");
                    var atualizarTicket = new
                    {
                        id = (int)contatoBlip.resource.extras.ticketIdMovidesk,
                        actions = new[]
                        {
                        new {
                            id = 0,
                            type= 2,
                            origin= 2,
                            description= respostaAtendente.Replace("\n","<br/>"),
                            status= "Resolvido",
                            baseStatus= "Resolved",
                            justification = null as string,
                            createdDate= DateTime.Now,
                            createdBy= new
                            {
                                id = (string)contatoBlip.resource.extras.ownerId,
                                personType = 1,
                                profileType = 3,
                                businessName = (string)contatoBlip.resource.extras.ownerBusinessName,
                                email = (string)contatoBlip.resource.extras.ownerEmail
                            },
                            isDeleted= false,
                            timeAppointments= new object[]{ },
                            attachments= new object[]{},
                            expenses= new object[]{},
                            tags= new object[] { }
                        }
                    }
                    };

                    string atualizarTicketJson = JsonSerializer.Serialize(atualizarTicket);
                    var atualizarTicketResponse = await _movideskApi.UpdateTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", (int)contatoBlip.resource.extras.ticketIdMovidesk, atualizarTicketJson);
                }
                else
                {
                    Console.WriteLine($"{contatoBlip.resource.extras.ticketIdMovidesk} - Ticket em andamento");

                    var atualizarTicket = new
                    {
                        id = (int)contatoBlip.resource.extras.ticketIdMovidesk,
                        actions = new[]
                        {
                            new {
                                id = 0,
                                type= 2,
                                origin= 2,
                                description = respostaAtendente.Replace("\n","<br/>"),
                                status= "Novo",
                                justification = (string)null,
                                createdDate= DateTime.Now,
                                createdBy= new
                                {
                                    id = (string)contatoBlip.resource.extras.ownerId,
                                    personType = 1,
                                    profileType = 3,
                                    businessName = (string)contatoBlip.resource.extras.ownerBusinessName,
                                    email = (string)contatoBlip.resource.extras.ownerEmail
                                },
                                isDeleted= false,
                                timeAppointments= new object[]{ },
                                attachments= new object[]{},
                                expenses= new object[]{},
                                tags= new object[] { }
                            }
                        }
                    };

                    string atualizarTicketJson = JsonSerializer.Serialize(atualizarTicket);
                    var atualizarTicketResponse = await _movideskApi.UpdateTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", (int)contatoBlip.resource.extras.ticketIdMovidesk, atualizarTicketJson);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
