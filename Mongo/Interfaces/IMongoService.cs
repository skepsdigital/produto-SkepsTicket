
using MongoDB.Bson;
using SkepsTicket.Model;
using SkepsTicket.Mongo.Model;

namespace SkepsTicket.Mongo.Interfaces
{
    public interface IMongoService
    {
        Task<SendBlipFilaMongo> BuscarItemNaFila();
        Task<List<EmailAtivoMongo>> BuscarTickets(string empresa, DateTime? startDate, DateTime? endDate, string? attendant, int page);
        Task CreateClientAsync(ClienteMongo cliente);
        Task CreateEmailAtivoAsync(EmailAtivoMongo emailAtivo);
        Task<ClienteMongo> GetByIdAsync(string email);
        Task InserirBlipCloseTicket(BlipCloseTicketResponse blipCloseTicket);
        Task InserirComandoNaFila(SendBlipFilaMongo sendBlipFila);
        Task InserirWebhookTicket(WebhookTicketMongo webhookTicket);
        Task UpdateAsync(ObjectId id, ClienteMongo clienteAtualizado);
    }
}
