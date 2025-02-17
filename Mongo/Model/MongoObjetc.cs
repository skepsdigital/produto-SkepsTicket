using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SkepsTicket.Mongo.Model
{
    public class MongoObjetc
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
