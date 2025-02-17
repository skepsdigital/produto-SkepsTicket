using Microsoft.Extensions.Options;
using RestEase;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Strategy.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SkepsTicket.Strategy
{
    public class FoodToSaveStrategy : ITicketStrategy
    {
        private readonly List<Empresa> _empresas;
        private const string URL_ANEXO_BASE = "https://zenvia.movidesk.com/Storage/Download?id=";

        public FoodToSaveStrategy(IOptions<EmpresasConfig> empresasConfig)
        {
            _empresas = empresasConfig.Value.Empresas;
        }

        public async Task Processar(TicketModel ticket)
        {
            var empresaKey = ticket.OriginEmailAccount ?? ticket.Owner.BusinessName;
            var empresa = _empresas.First(e => e.OwnerBusinessName.Equals(empresaKey) || e.OriginEmailAccount.Equals(empresaKey));

            Console.WriteLine($"{ticket.Id} - Cliente encontrado - {ticket.Clients.First().Email}");

            var contatoIdentity = ticket.Clients.First().Email.Replace("@", "%40") + $".{empresa.Bot}@0mn.io";

            var blipSender = RestClient.For<IBlipSender>($"https://{empresa.Contrato}.http.msging.net");
            var sendMessageBlip = RestClient.For<ISendMessageBlip>("https://skepsticket-node.azurewebsites.net");

            var idsProcessados = new List<string>();

            try
            {
                string requestContatoJson = JsonSerializer.Serialize(new BlipCommandModel { Id = Guid.NewGuid().ToString(), Method = "get", To = "postmaster@crm.msging.net", Uri = $"/contacts/{contatoIdentity.Replace("%40", "%2540")}" });
                var contatoBlip = await blipSender.SendCommandAsync(requestContatoJson, empresa.Key);

                if (contatoBlip.status != "success")
                {
                    string requestAccountJson = JsonSerializer.Serialize(new { email = ticket.Clients.First().Email.Replace("@", "%40"), identificadorBot = empresa.Bot });
                    var criarAccountResponse = await sendMessageBlip.CriarAccount(requestAccountJson);

                    string requesteCriarContaJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                    {
                        Resource = new Resource
                        {
                            Identity = contatoIdentity,
                            Extras = new Dictionary<string, string>
                            {
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", ticket.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            }
                        }
                    });

                    var criarContatoResponse = await blipSender.SendContactAsync(requesteCriarContaJson, empresa.Key);
                }
                else
                {
                    var intsListaSeparadaPorVirgula = (string)contatoBlip.resource.extras.idsProcessados;
                    idsProcessados = intsListaSeparadaPorVirgula.Split(',')
                                               .Where(id => !string.IsNullOrWhiteSpace(id))
                                               .ToList();
                }

                foreach (var action in ticket.Actions.Where(a => a.Origin == 3 && !idsProcessados.Contains(a.CreatedDate?.Ticks.ToString())))
                {
                    var mensagem = Regex.Replace(action.Description, @"Em (seg|ter|qua|qui|sex|sab|dom)\., \d{1,2} de \w{3}\. de \d{4}.*", string.Empty, RegexOptions.Singleline);

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
                    await sendMessageBlip.EnviarMensagem(requestEnviarMsgJson);

                    idsProcessados.Add(action.CreatedDate?.Ticks.ToString());
                }

                string requesteAtualizarContatoJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                {
                    Method = "merge",
                    Resource = new Resource
                    {
                        Identity = contatoIdentity,
                        Extras = new Dictionary<string, string>
                            {
                                { "idsProcessados", string.Join(",", idsProcessados) },
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", ticket.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            }
                    }
                });

                var atualizarContatoResponse = await blipSender.SendContactAsync(requesteAtualizarContatoJson, empresa.Key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ticket.Id} -> Erro ao processar ticket");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ProcessarAtivo(dynamic dynamic)
        {

        }
    }
}
