using MongoDB.Driver;
using Restaurante.Data.Schemas;
using Restaurante.Domain.Entities;

namespace Restaurante.Data.Repositories
{
    public class RestauranteRepository
    {
        IMongoCollection<RestaurantSchema> _restaurantes;
        public RestauranteRepository(MongoDB mongoDB)
        {
            _restaurantes = mongoDB.DB.GetCollection<RestaurantSchema>("restaurantes");
        }

        public void Inserir(Restaurant restaurante)
        {
            var document = new RestaurantSchema()
            {
                Nome = restaurante.Nome,
                Cozinha = restaurante.Cozinha,

                Endereco = new EnderecoSchema()
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    Cep = restaurante.Endereco.Cep,
                    UF = restaurante.Endereco.UF
                }
            };
            _restaurantes.InsertOne(document);
        }
    }
}
