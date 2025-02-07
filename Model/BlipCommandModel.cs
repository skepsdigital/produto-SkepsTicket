using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class BlipCommandModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; } 

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
