using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SkepsTicket.Model;

namespace SkepsTicket.Mongo.Model
{
    public class LoginMongo : LoginModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime Validate { get; set; }
    }
}
