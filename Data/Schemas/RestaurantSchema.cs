using MongoDB.Bson;
using Restaurante.Domain.Enums;

namespace Restaurante.Data.Schemas
{
    public class RestaurantSchema
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public EnderecoSchema Endereco { get; set; }
    }
}
