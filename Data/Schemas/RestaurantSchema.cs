using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Restaurante.Domain.Entities;
using Restaurante.Domain.Enums;
using Restaurante.Domain.ValueObjects;

namespace Restaurante.Data.Schemas
{
    public class RestaurantSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public EnderecoSchema Endereco { get; set; }
        public void AtribuirEndereco(EnderecoSchema endereco)
        {
            Endereco = endereco;
        }
    }
    public static class RestaurantSchemaExtensao
    {
            public static Restaurant ConverterParaDomain(this RestaurantSchema document)
        {
            var restaurante = new Restaurant(document.Nome, document.Cozinha);
            var endereco = new Endereco(document.Endereco.Logradouro, document.Endereco.Numero, document.Endereco.Cidade, document.Endereco.UF, document.Endereco.Cep);
            restaurante.AtribuirEndereco(endereco);
            restaurante.Id = document.Id;
            return restaurante;
        }
    }
}
