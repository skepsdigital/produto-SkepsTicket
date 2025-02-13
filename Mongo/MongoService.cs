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

        public MongoService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Produtos");

            _clienteCollection = database.GetCollection<ClienteMongo>("SkepsTicket_user");
            _emailAtivoCollection = database.GetCollection<EmailAtivoMongo>("SkepsTicket_emailAtivo");
            _sendBlipFilaCollection = database.GetCollection<SendBlipFilaMongo>("SkepsTicket_sendBlipFila");
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

        public async Task<List<EmailAtivoMongo>> BuscarTickets(string empresa, DateTime? startDate, DateTime? endDate, string? attendant)
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

            return await _emailAtivoCollection.Find(finalFilter).ToListAsync();
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
