
using MongoDB.Bson;
using SkepsTicket.Mongo.Model;

namespace SkepsTicket.Mongo.Interfaces
{
    public interface IMongoService
    {
        Task CreateClientAsync(ClienteMongo cliente);
        Task CreateEmailAtivoAsync(EmailAtivoMongo emailAtivo);
        Task<ClienteMongo> GetByIdAsync(string email);
        Task UpdateAsync(ObjectId id, ClienteMongo clienteAtualizado);
    }
}
