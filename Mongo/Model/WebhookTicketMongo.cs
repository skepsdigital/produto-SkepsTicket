using SkepsTicket.Model;

namespace SkepsTicket.Mongo.Model
{
    public class WebhookTicketMongo: MongoObjetc
    {
        public string TicketMovideskId { get; set; }
        public TicketModel? ticket { get; set; }
    }
}
