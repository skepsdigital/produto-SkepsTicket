using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class CriarContatoBlipModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("to")]
        public string To { get; set; } = "postmaster@crm.msging.net";

        [JsonPropertyName("method")]
        public string Method { get; set; } = "set";

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = "/contacts";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "application/vnd.lime.contact+json";

        [JsonPropertyName("resource")]
        public Resource Resource { get; set; } // Objeto Resource
    }

    public class Resource
    {
        [JsonPropertyName("identity")]
        public string Identity { get; set; }
        public Dictionary<string, string> Extras { get; set; } // Extras

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
