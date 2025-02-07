using MongoDB.Driver;
using System.Text.Json;
using MongoDB.Bson;
using SkepsTicket.Mongo.Interfaces;
using SkepsTicket.Mongo.Model;
namespace SkepsTicket.Mongo
{
    public class MongoService : IMongoService
    {
        private readonly IMongoCollection<ClienteMongo> _clienteCollection;
        private readonly IMongoCollection<EmailAtivoMongo> _emailAtivoCollection;

        public MongoService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Produtos");

            _clienteCollection = database.GetCollection<ClienteMongo>("SkepsTicket_user");
            _emailAtivoCollection = database.GetCollection<EmailAtivoMongo>("SkepsTicket_emailAtivo");
        }

        public async Task CreateClientAsync(ClienteMongo cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "O objeto cliente não pode ser nulo.");
            }

            await _clienteCollection.InsertOneAsync(cliente);
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
