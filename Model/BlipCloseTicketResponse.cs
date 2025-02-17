using SkepsTicket.Mongo.Model;

namespace SkepsTicket.Model
{
    public class BlipCloseTicketResponse : MongoObjetc
    {
        public string BlipTicketId { get; set; }
        public string BotKey { get; set; }
        public string Contrato { get; set; }
        public string Identity { get; set; }
        public string Tags {  get; set; }
    }
}
