using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Text.Json;

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
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DataCriação { get; set; }

        [JsonPropertyName("linkHistory")]
        public string LinkBlip { get; set; }

        [JsonPropertyName("clientID")]
        public string Empresa { get; set; }

        [JsonPropertyName("category")]
        public string Categoria { get; set; }

        public int Total { get; set; }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "dd/MM/yyyy HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
