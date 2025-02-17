using Microsoft.Extensions.Options;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Strategy.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SkepsTicket.Strategy
{
    public class PixBetStrategy : ITicketStrategy
    {
        private readonly List<Empresa> _empresas;
        private const string URL_ANEXO_BASE = "https://zenvia.movidesk.com/Storage/Download?id=";
        private readonly ISendMessageBlip _sendMessageBlip;
        private readonly Func<string, IBlipSender> _blipSender;
        private readonly IMongoService _mongoService;

        public PixBetStrategy(IOptions<EmpresasConfig> empresasConfig, ISendMessageBlip sendMessageBlip, Func<string, IBlipSender> defaultClient, IMongoService mongoService)
        {
            _empresas = empresasConfig.Value.Empresas;
            _sendMessageBlip = sendMessageBlip;
            _blipSender = defaultClient;
            _mongoService = mongoService;
        }

        public async Task Processar(TicketModel ticket)
        {
            var empresaKey = ticket.OriginEmailAccount ?? ticket.Owner.BusinessName;
            var empresa = _empresas.First(e => e.OwnerBusinessName.Equals(empresaKey) || e.OriginEmailAccount.Equals(empresaKey));

            Console.WriteLine($"{ticket.Id} - Cliente encontrado - {ticket.Clients.First().Email}");

            var contatoIdentity = ticket.Clients.First().Email.Replace("@", "%40") + $".{empresa.Bot}@0mn.io";

            var blipSender = _blipSender($"https://{empresa.Contrato}.http.msging.net");

            var idsProcessados = new List<string>();

            try
            {
                string requestContatoJson = JsonSerializer.Serialize(new BlipCommandModel { Id = Guid.NewGuid().ToString(), Method = "get", To = "postmaster@crm.msging.net", Uri = $"/contacts/{contatoIdentity.Replace("%40", "%2540")}" });
                var contatoBlip = await blipSender.SendCommandAsync(requestContatoJson, empresa.Key);

                if (contatoBlip.status != "success")
                {
                    string requesteCriarContaJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                    {
                        Resource = new Resource
                        {
                            Name = ticket.Clients.First().BusinessName,
                            Identity = contatoIdentity,
                            Extras = new Dictionary<string, string>
                            {
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", ticket.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            },
                            Source = "0mn.io"
                        }
                    });

                    Console.WriteLine($"{ticket.Id} -> Criando contato - {requesteCriarContaJson} - {empresa.OwnerBusinessName}");

                    var criarContatoResponse = await blipSender.SendContactAsync(requesteCriarContaJson, empresa.Key);

                    Console.WriteLine($"{ticket.Id} -> Criar contato Status - {criarContatoResponse.status}");

                    string requestAccountJson = JsonSerializer.Serialize(new { email = ticket.Clients.First().Email.Replace("@", "%40"), identificadorBot = empresa.Bot, contrato = empresa.Contrato });
                    var criarAccountResponse = await _sendMessageBlip.CriarAccount(requestAccountJson);
                    Console.WriteLine($"{ticket.Id}- {criarAccountResponse}");
                }
                else
                {
                    Console.WriteLine($"{ticket.Id} -> Contato ja existente - {contatoIdentity}");
                    try
                    {
                        var intsListaSeparadaPorVirgula = (string)contatoBlip.resource.extras.idsProcessados;
                        idsProcessados = intsListaSeparadaPorVirgula.Split(',')
                                                   .Where(id => !string.IsNullOrWhiteSpace(id))
                                                   .ToList();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"{ticket.Id} -> Sem ID Processado - {contatoIdentity}");
                    }
                }

                var executouAlgumAcao = false;

                if (ticket.Actions.Last().Origin != 3 && (ticket.Tags is null || !ticket.Tags.Any()))
                {
                    Console.WriteLine($"{ticket.Id} - Email Ativo na tela antiga");
                }
                else
                {
                    foreach (var action in ticket.Actions.Where(a => a.Origin == 3 && !idsProcessados.Contains(a.CreatedDate?.Ticks.ToString())))
                    {

                        var mensagem = Regex.Replace(action.Description, @"#####.*", string.Empty, RegexOptions.Singleline);
                        mensagem = Regex.Replace(mensagem, @"Em (seg|ter|qua|qui|sex|sab|sáb|dom|seg.|ter.|qua.|qui.|sex.|sab.|sáb.|dom.),? .*", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        var anexoFragmento = Regex.Matches(action.HtmlDescription, @"\/([A-Fa-f0-9]{32})(?=\?)")
                            .Cast<Match>()
                            .Where(match => match.Success)
                            .Select(match => match.Groups[1].Value)
                            .ToList();

                        if (action.Attachments is not null)
                        {
                            anexoFragmento.AddRange(action.Attachments.Select(a => a.Path));
                        }

                        mensagem += string.Join(Environment.NewLine, anexoFragmento.Distinct().Select(anx => $"{URL_ANEXO_BASE}{anx}"));

                        string requestEnviarMsgJson = JsonSerializer.Serialize(new { email = ticket.Clients.First().Email.Replace("@", "%40"), mensagem = mensagem, identificadorBot = empresa.Bot, contrato = empresa.Contrato });
                        string requestAccountJson = JsonSerializer.Serialize(new { email = ticket.Clients.First().Email.Replace("@", "%40"), identificadorBot = empresa.Bot, contrato = empresa.Contrato });

                        var sendBlipFila = new SendBlipFilaMongo
                        {
                            EnviarMensagem = requestEnviarMsgJson,
                            CriarContaJson = requestAccountJson
                        };

                        await _mongoService.InserirComandoNaFila(sendBlipFila);

                        Console.WriteLine($"{ticket.Id} - Comandos inseridos na fila");

                        idsProcessados.Add(action.CreatedDate?.Ticks.ToString());

                        executouAlgumAcao = true;
                    }
                }

                if (executouAlgumAcao)
                {
                    string requesteAtualizarContatoJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                    {
                        Method = "merge",
                        Resource = new Resource
                        {
                            Identity = contatoIdentity,
                            Name = ticket.Clients.First().BusinessName,
                            Extras = new Dictionary<string, string>
                            {
                                { "idsProcessados", string.Join(",", idsProcessados) },
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", ticket.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            },
                            Source = "0mn.io"
                        }
                    });

                    var atualizarContatoResponse = await blipSender.SendContactAsync(requesteAtualizarContatoJson, empresa.Key);
                    Console.WriteLine($"{ticket.Id} -> Atualizar contato Status - {atualizarContatoResponse.status}");
                }

                Console.WriteLine($"{ticket.Id} - PROCESSO FINALIZADO");
            }
            catch (Exception ex)
            {
                await _mongoService.InserirWebhookTicket(new WebhookTicketMongo
                {
                    ticket = ticket,
                    TicketMovideskId = ticket.Id

                });
                Console.WriteLine($"{ticket.Id} -> Erro ao processar ticket");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ProcessarAtivo(dynamic dynamic)
        {

        }
    }
}
