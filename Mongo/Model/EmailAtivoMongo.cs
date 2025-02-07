using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SkepsTicket.Mongo.Model
{
    public class EmailAtivoMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [JsonPropertyName("receiverMail")]
        public string Destinatario { get; set; }

        [JsonPropertyName("subject")]
        public string Assunto { get; set; }

        [JsonPropertyName("attendantMail")]
        public string Atendente { get; set; }

        [JsonPropertyName("chatHistory")]
        public string Conteudo { get; set; }

        [JsonPropertyName("date")]
        public DateTime DataCriação { get; set; }

        [JsonPropertyName("linkHistory")]
        public string LinkBlip { get; set; }
    }
}
