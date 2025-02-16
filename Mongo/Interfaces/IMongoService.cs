﻿
using MongoDB.Bson;
using SkepsTicket.Mongo.Model;

namespace SkepsTicket.Mongo.Interfaces
{
    public interface IMongoService
    {
        Task<SendBlipFilaMongo> BuscarItemNaFila();
        Task<List<EmailAtivoMongo>> BuscarTickets(string empresa, DateTime? startDate, DateTime? endDate, string? attendant);
        Task CreateClientAsync(ClienteMongo cliente);
        Task CreateEmailAtivoAsync(EmailAtivoMongo emailAtivo);
        Task<ClienteMongo> GetByIdAsync(string email);
        Task InserirComandoNaFila(SendBlipFilaMongo sendBlipFila);
        Task UpdateAsync(ObjectId id, ClienteMongo clienteAtualizado);
    }
}
