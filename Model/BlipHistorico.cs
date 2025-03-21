using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class BlipHistorico
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("resource")]
        public ResourceHistorico Resource { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class ResourceHistorico
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("itemType")]
        public string ItemType { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("content")]
        public object Content { get; set; } // Use `object` to handle both strings and nested objects.

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("traceparent")]
        public string Traceparent { get; set; }

        [JsonPropertyName("#command.uri")]
        public string CommandUri { get; set; }
    }

    public class TicketContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("sequentialId")]
        public int SequentialId { get; set; }

        [JsonPropertyName("ownerIdentity")]
        public string OwnerIdentity { get; set; }

        [JsonPropertyName("customerIdentity")]
        public string CustomerIdentity { get; set; }

        [JsonPropertyName("customerDomain")]
        public string CustomerDomain { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("storageDate")]
        public DateTime StorageDate { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("team")]
        public string Team { get; set; }

        [JsonPropertyName("unreadMessages")]
        public int UnreadMessages { get; set; }

        [JsonPropertyName("closed")]
        public bool Closed { get; set; }

        [JsonPropertyName("customerInput")]
        public CustomerInput CustomerInput { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }
    }

    public class Media
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

    public class CustomerInput
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}