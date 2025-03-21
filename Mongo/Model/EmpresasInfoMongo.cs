using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SkepsTicket.Mongo.Model
{
    public class EmpresasInfoMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Empresa { get; set; }
        public List<string> Atendentes { get; set; }
        public Dictionary<string, string> Categorias { get; set; }
    }
}
