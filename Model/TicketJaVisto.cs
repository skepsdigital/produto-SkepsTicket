using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SkepsTicket.Model
{
    public class TicketJaVisto
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string TicketId { get; set; }
    }
}
