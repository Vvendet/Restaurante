using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Restaurante.Domain.Enums;

namespace Restaurante.Data.Schemas
{
    public class RestaurantSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public EnderecoSchema Endereco { get; set; }
    }
}
