using MongoDB.Driver;
using System.Text.Json;
using MongoDB.Bson;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
using SkepsTicket.Model;
namespace SkepsTicket.Mongo
{
    public class MongoService : IMongoService
    {
        private readonly IMongoCollection<ClienteMongo> _clienteCollection;
        private readonly IMongoCollection<EmailAtivoMongo> _emailAtivoCollection;
        private readonly IMongoCollection<SendBlipFilaMongo> _sendBlipFilaCollection;
        private readonly IMongoCollection<BlipCloseTicketResponse> _blipCloseTicketResponse;
        private readonly IMongoCollection<WebhookTicketMongo> _webhookTicketMongo;
        private readonly IMongoCollection<TicketJaVisto> ticketmongo;
        private readonly IMongoCollection<LoginMongo> _loginCollection;
        private readonly IMongoCollection<EmpresasInfoMongo> _empresasInfoCollection;

        private const int MAX_TICKET_PAGE = 10;

        public MongoService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Produtos");

            _clienteCollection = database.GetCollection<ClienteMongo>("SkepsTicket_user");
            _emailAtivoCollection = database.GetCollection<EmailAtivoMongo>("SkepsTicket_emailAtivo");
            _sendBlipFilaCollection = database.GetCollection<SendBlipFilaMongo>("SkepsTicket_sendBlipFila");
            _blipCloseTicketResponse = database.GetCollection<BlipCloseTicketResponse>("SkepsTicket_blipCloseTicket");
            _webhookTicketMongo = database.GetCollection<WebhookTicketMongo>("SkepsTicket_webhookticket");
            ticketmongo = database.GetCollection<TicketJaVisto>("SkepsTicket_ticketsjaprocessados");
            _loginCollection = database.GetCollection<LoginMongo>("SkepsTicket_loginCode");
            _empresasInfoCollection = database.GetCollection<EmpresasInfoMongo>("SkepsTicket_empresasInfo");

        }

        public async Task<List<EmpresasInfoMongo>> GetEmpresasInfoAsync()
        {
            return await _empresasInfoCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task InserirEmpresasInfo(EmpresasInfoMongo empresas) => await _empresasInfoCollection.InsertOneAsync(empresas);

        public async Task InserirLoginCode(LoginMongo loginMongo)
        {
            await _loginCollection.InsertOneAsync(loginMongo);
        }

        public async Task<LoginMongo> GetLoginCodeByCodeAsync(string code)
        {
            var filter = Builders<LoginMongo>.Filter.Eq(c => c.Code, code);
            return await _loginCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveLoginCodeAsync(string code)
        {
            var filter = Builders<LoginMongo>.Filter.Eq(c => c.Code, code);
            var result = await _loginCollection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        public async Task InserirWebhookTicket(WebhookTicketMongo webhookTicket)
        {
            await _webhookTicketMongo.InsertOneAsync(webhookTicket);
        }

        public async Task InserirTicketJaVisto(TicketJaVisto ticket)
        {
            await ticketmongo.InsertOneAsync(ticket);
        }

        public async Task<TicketJaVisto> GetByIdTicketAsync(string ticketId)
        {
            var filter = Builders<TicketJaVisto>.Filter.Eq(c => c.TicketId, ticketId);
            return await ticketmongo.Find(filter).FirstOrDefaultAsync();
        }
        public async Task InserirBlipCloseTicket(BlipCloseTicketResponse blipCloseTicket)
        {
            await _blipCloseTicketResponse.InsertOneAsync(blipCloseTicket);
        }

        public async Task InserirComandoNaFila(SendBlipFilaMongo sendBlipFila)
        {
            await _sendBlipFilaCollection.InsertOneAsync(sendBlipFila);
        }

        public async Task<SendBlipFilaMongo> BuscarItemNaFila()
        {
            // Encontra o primeiro item na fila, ordenado pelo ObjectId (para pegar o mais antigo)
            var item = await _sendBlipFilaCollection
                .Find(FilterDefinition<SendBlipFilaMongo>.Empty)
                .Sort(Builders<SendBlipFilaMongo>.Sort.Ascending(doc => doc.Id))
                .Limit(1)
                .FirstOrDefaultAsync();

            if (item != null)
            {
                // Remove o item após ser encontrado
                await _sendBlipFilaCollection.DeleteOneAsync(Builders<SendBlipFilaMongo>.Filter.Eq("_id", item.Id));
            }

            return item;
        }
        public async Task CreateClientAsync(ClienteMongo cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "O objeto cliente não pode ser nulo.");
            }

            await _clienteCollection.InsertOneAsync(cliente);
        }

        public async Task<List<EmailAtivoMongo>> BuscarTickets(string empresa, DateTime? startDate, DateTime? endDate, string? attendant, int page)
        {
            var filterBuilder = Builders<EmailAtivoMongo>.Filter;
            var filters = new List<FilterDefinition<EmailAtivoMongo>>();

            if (!string.IsNullOrEmpty(empresa))
                filters.Add(filterBuilder.Eq(e => e.Empresa, empresa));

            if (startDate.HasValue)
                filters.Add(filterBuilder.Gte(e => e.DataCriação, startDate.Value));

            if (endDate.HasValue)
                filters.Add(filterBuilder.Lte(e => e.DataCriação, endDate.Value));

            if (!string.IsNullOrEmpty(attendant))
                filters.Add(filterBuilder.Eq(e => e.Atendente, attendant));

            var finalFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            return await _emailAtivoCollection
                .Find(finalFilter)
                .SortByDescending(e => e.Id)
                .Skip((page - 1) * MAX_TICKET_PAGE)
                .Limit(MAX_TICKET_PAGE)
                .ToListAsync();
        }

        public async Task CreateEmailAtivoAsync(EmailAtivoMongo emailAtivo)
        {
            if (emailAtivo == null)
            {
                throw new ArgumentNullException(nameof(emailAtivo), "O objeto cliente não pode ser nulo.");
            }

            await _emailAtivoCollection.InsertOneAsync(emailAtivo);
        }

        public async Task UpdateAsync(ObjectId id, ClienteMongo clienteAtualizado)
        {
            var filter = Builders<ClienteMongo>.Filter.Eq(c => c.Id, id);

            var updateResult = await _clienteCollection.ReplaceOneAsync(filter, clienteAtualizado);

            if (updateResult.MatchedCount == 0)
            {
                throw new Exception($"Nenhum documento encontrado com o ID: {id}.");
            }
        }

        public async Task<ClienteMongo> GetByIdAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "O ID não pode ser nulo ou vazio.");
            }

            var filter = Builders<ClienteMongo>.Filter.Eq(c => c.Email, email);
            return await _clienteCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
