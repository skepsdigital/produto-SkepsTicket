using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SkepsTicket.Mongo.Model
{
    public class SendBlipFilaMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string CriarContaJson { get; set; }
        public string EnviarMensagem { get; set; }
    }
}
