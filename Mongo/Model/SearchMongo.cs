using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SkepsTicket.Mongo.Model
{
    public class SearchMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string AttendantEmail { get; set; }
        public string RatingNote { get; set; }
        public string Suggestion { get; set; }
    }
}

