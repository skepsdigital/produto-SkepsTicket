using Microsoft.Extensions.Options;
using RestEase;
using SkepsTicket.Infra.RestEase;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Services.Interfaces;
using SkepsTicket.Strategy;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkepsTicket.Services
{
    public class MovideskService : IMovideskService
    {
        private readonly ILogger<MovideskService> _logger;
        private readonly IMovideskAPI _movideskAPI;
        private readonly IMongoService _mongoService;
        private readonly TicketStrategyFactory strategyFactory;
        private readonly List<Empresa> _empresas;
        private const string EmailNaoResponda = "naoresponda@pixbet.com,naoresponda@pixbet.com.br,naoresponda@betdasorte.io";

        public MovideskService(ILogger<MovideskService> logger, IMovideskAPI movideskAPI, TicketStrategyFactory strategyFactory, IMongoService mongoService, IOptions<EmpresasConfig> empresasConfig)
        {
            _logger = logger;
            _movideskAPI = movideskAPI;
            this.strategyFactory = strategyFactory;
            _mongoService = mongoService;
            _empresas = empresasConfig.Value.Empresas;
        }

        private async Task CriarOuAtualizarCliente(string emailCliente, string? nome, string empresa, string ticketId)
        {
            var cliente = await _mongoService.GetByIdAsync(emailCliente);
            if (cliente is null)
            {
                cliente = new ClienteMongo()
                {
                    Email = emailCliente,
                    Nome = nome ?? emailCliente,
                    TicketsMovidesk = new List<string>()
                        {
                            ticketId
                        },
                    Empresa = empresa,
                    UltimaIteracao = DateTime.UtcNow
                };

                await _mongoService.CreateClientAsync(cliente);
            }
            else if (!cliente.TicketsMovidesk.Contains(ticketId))
            {
                cliente.TicketsMovidesk.Add(ticketId);
                cliente.UltimaIteracao = DateTime.UtcNow;

                await _mongoService.UpdateAsync(cliente.Id, cliente);
            }
        }

        public async Task ProcessarNovoTicketReceptivo(string ticketMovideskId)
        {
            try
            {
                var ticket = await _movideskAPI.GetTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", int.Parse(ticketMovideskId));
                var strategyKey = ticket.OriginEmailAccount ?? ticket.Owner.BusinessName;

                Console.WriteLine($"INFORMACOES TICKET - {ticketMovideskId} - {ticket.ActionCount} - {ticket.OriginEmailAccount} - {ticket.Category} - {ticket.Owner.BusinessName} - {ticket.Clients.Count}");


                if (!string.IsNullOrWhiteSpace(EmailNaoResponda))
                {
                    var listaEmailNaoResponda = EmailNaoResponda.Split(",");

                    if (ticket.Clients.Select(c => c.Email).Intersect(listaEmailNaoResponda).Any())
                    {
                        ticket.Clients.First().Email = ticket.Cc;
                        ticket.Clients.First().BusinessName = ticket.Cc;
                    }
                }

                var emailCliente = ticket.Clients.FirstOrDefault().Email;
                await CriarOuAtualizarCliente(emailCliente, ticket.Clients.First().BusinessName, strategyKey, ticketMovideskId);

                Console.WriteLine($"{ticketMovideskId} - {strategyKey}");

                var ticketStrategy = strategyFactory.GetStrategy(strategyKey);

                await ticketStrategy.Processar(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar o ticket - {ticketMovideskId}");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task TranscreverTicket(TranscreverTicketModel transcrever)
        {
            var empresa = _empresas.First(e => e.Categoria.Equals(transcrever.Category));

            var cliente = await _movideskAPI.GetClientAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", $"Emails/any(e: e/email eq '{transcrever.Email}')");
            var idCliente = string.Empty;

            if (!cliente.Any())
            {
                var criarClienteMovidesk = new
                {
                    isActive = true,
                    personType = 1,
                    profileType = 2,
                    businessName = transcrever.Email,
                    emails = new[]
                    {
                          new
                          {
                              emailType = "Comercial",
                              email = transcrever.Email,
                              isDefault = true
                          }
                    }
                };

                var novoCliente = await _movideskAPI.CreateClientAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", JsonSerializer.Serialize(criarClienteMovidesk));
                idCliente = novoCliente.id;
            }
            else
            {
                idCliente = cliente.First().id;
            }

            var novoTicket = new TicketModel()
            {
                Type = 2,
                Subject = transcrever.Subject,
                Category = transcrever.CategoriaTranscricaoEmail,
                OwnerTeam = empresa.OwnerTeam,
                Cc = transcrever.CC,
                OriginEmailAccount = empresa.OriginEmailAccount,
                Actions = new List<Model.Action>()
                {
                    new Model.Action()
                    {
                        Id = 1,
                        Type = 2,
                        Origin = 3,
                        Description = transcrever.Description,
                        Status = "Novo",
                        CreatedBy = new CreatedBy
                        {
                            Id = idCliente,
                            PersonType = 1,
                            ProfileType = 2,
                            Email = transcrever.Email
                        },
                        IsDeleted = false,
                        TimeAppointments = new List<object>(),
                        Attachments = new List<Attachments>(),
                        Expenses = new List<object>(),
                        Tags = new List<string>()
                    }
                },
                Owner = new Owner
                {
                    Id = empresa.OwnerId,
                    PersonType = 1,
                    ProfileType = 3,
                    BusinessName = empresa.OwnerBusinessName,
                    Email = empresa.OwnerEmail
                },
                CreatedBy = new CreatedBy
                {
                    Id = empresa.OwnerId,
                    PersonType = 1,
                    ProfileType = 3,
                    BusinessName = empresa.OwnerBusinessName,
                    Email = empresa.OwnerEmail
                },
                Clients = new List<Client>()
                {
                    new Client()
                    {
                        Id = idCliente,
                        PersonType = 1,
                        ProfileType = 2
                    }
                }
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var novoTicketJson = JsonSerializer.Serialize(novoTicket, options);
            var novoTicketResponse = await _movideskAPI.CreateTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", 1, novoTicketJson);
        }

        public async Task<string> CriarTicketAtivo(EmailAtivoModel emailAtivo)
        {
            var empresa = _empresas.First(e => e.Categoria.Equals(emailAtivo.Category));

            var emailAtivoMongo = new EmailAtivoMongo
            {
                Assunto = emailAtivo.Subject,
                Atendente = emailAtivo.Attendant,
                Conteudo = emailAtivo.Description,
                DataCriação = DateTime.UtcNow.AddHours(-3),
                Destinatario = emailAtivo.Email,
                Empresa = emailAtivo.ClientID,
                Categoria = emailAtivo.Category
            };

            await _mongoService.CreateEmailAtivoAsync(emailAtivoMongo);

            var cliente = await _movideskAPI.GetClientAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", $"Emails/any(e: e/email eq '{emailAtivo.Email}')");
            var idCliente = string.Empty;

            if(!cliente.Any())
            {
                var criarClienteMovidesk = new
                {
                    isActive = true,
                    personType = 1,
                    profileType = 2,
                    businessName = emailAtivo.Name,
                    emails = new[]
                    {
                          new
                          {
                              emailType = "Comercial",
                              email = emailAtivo.Email,
                              isDefault = true
                          }
                    }
                };

                var novoCliente = await _movideskAPI.CreateClientAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7", JsonSerializer.Serialize(criarClienteMovidesk));
                idCliente = novoCliente.id;
            }
            else
            {
                idCliente = cliente.First().id;
            }

            var novoTicket = new TicketModel()
            {
                Type = 2,
                Subject = "Novo protocolo: " + emailAtivo.Subject,
                Category = empresa.CategoriaEmailAtivo,
                OwnerTeam = empresa.OwnerTeam,
                Cc = emailAtivo.CC ?? string.Empty,
                OriginEmailAccount = empresa.OriginEmailAccount,
                Actions = new List<Model.Action>()
                {
                    new Model.Action()
                    {
                        Id = 1,
                        Type = 2,
                        Origin = 2,
                        Description = emailAtivo.Description,
                        Status = "Novo",
                        CreatedBy = new CreatedBy
                        {
                            Id = empresa.OwnerId,
                            PersonType = 1,
                            ProfileType = 3,
                            BusinessName = empresa.OwnerBusinessName,
                            Email = empresa.OwnerEmail
                        },
                        IsDeleted = false,
                        TimeAppointments = new List<object>(),
                        Attachments = new List<Attachments>(),
                        Expenses = new List<object>(),
                        Tags = new List<string>()
                    }
                },
                Owner = new Owner
                {
                    Id = empresa.OwnerId,
                    PersonType = 1,
                    ProfileType = 3,
                    BusinessName = empresa.OwnerBusinessName,
                    Email = empresa.OwnerEmail
                },
                CreatedBy = new CreatedBy
                {
                    Id = empresa.OwnerId,
                    PersonType = 1,
                    ProfileType = 3,
                    BusinessName = empresa.OwnerBusinessName,
                    Email = empresa.OwnerEmail
                },
                Clients = new List<Client>()
                {
                    new Client()
                    {
                        Id = idCliente,
                        PersonType = 1,
                        ProfileType = 2
                    }
                },
                Tags = new List<string>
                {
                    "SkepsTickets"
                }
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var novoTicketJson = JsonSerializer.Serialize(novoTicket, options);
            var novoTicketResponse = await _movideskAPI.CreateTicketAsync("e894e231-a6c0-4cc1-ab75-29ce219b5bd7",1, novoTicketJson);

            if(string.IsNullOrWhiteSpace(novoTicketResponse.Id))
            {
                return string.Empty;
            }

            await CriarOuAtualizarCliente(emailAtivo.Email, emailAtivo.Email, empresa.OwnerTeam, novoTicketResponse.Id);

            var contatoIdentity = emailAtivo.Email.Replace("@", "%40") + $".{empresa.Bot}@0mn.io";

            var blipSender = RestClient.For<IBlipSender>($"https://{empresa.Contrato}.http.msging.net");
            var sendMessageBlip = RestClient.For<ISendMessageBlip>("https://skepsticket-node.azurewebsites.net");

            string requestContatoJson = JsonSerializer.Serialize(new BlipCommandModel { Id = Guid.NewGuid().ToString(), Method = "get", To = "postmaster@crm.msging.net", Uri = $"/contacts/{contatoIdentity.Replace("%40", "%2540")}" });
            var contatoBlip = await blipSender.SendCommandAsync(requestContatoJson, empresa.Key);

            if (contatoBlip.status != "success")
            {
                string requestAccountJson = JsonSerializer.Serialize(new { email = emailAtivo.Email.Replace("@", "%40"), identificadorBot = empresa.Bot });
                var criarAccountResponse = await sendMessageBlip.CriarAccount(requestAccountJson);

                string requesteCriarContaJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                {
                    Resource = new Resource
                    {
                        Identity = contatoIdentity,
                        Name = emailAtivo.Name,
                        Extras = new Dictionary<string, string>
                            {
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", novoTicketResponse.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            },
                        Source = "0mn.io"
                    }
                });

                var criarContatoResponse = await blipSender.SendContactAsync(requesteCriarContaJson, empresa.Key);
            }
            else
            {
                string requesteCriarContaJson = JsonSerializer.Serialize(new CriarContatoBlipModel
                {
                    Method = "merge",
                    Resource = new Resource
                    {
                        Name = emailAtivo.Name,
                        Identity = contatoIdentity,
                        Extras = new Dictionary<string, string>
                            {
                                { "team", empresa.Categoria },
                                { "ticketIdMovidesk", novoTicketResponse.Id },
                                { "ownerId",  empresa.OwnerId},
                                { "ownerBusinessName", empresa.OwnerBusinessName},
                                { "ownerEmail", empresa.OwnerEmail}
                            },
                        Source = "0mn.io"
                    }
                });

                var criarContatoResponse = await blipSender.SendContactAsync(requesteCriarContaJson, empresa.Key);
            }

            var enviarMensagemAtendenteJson = JsonSerializer.Serialize(new
            {
                id = Guid.NewGuid().ToString(),
                to = contatoIdentity,
                type = "text/plain",
                content = emailAtivo.Description
            });

            var enviarMensagemAtendenteResponse = await blipSender.SendCommandMessageAsync(enviarMensagemAtendenteJson, empresa.Key);
            return novoTicketResponse.Id;
        }
    }
}
