using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SkepsTicket.Mongo.Model
{
    public class ClienteMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Email { get; set; }
        public string Nome { get; set; }
        public List<string> TicketsMovidesk { get; set; }
        public string Empresa { get; set; }
        public DateTime UltimaIteracao { get; set; }
    }
}

