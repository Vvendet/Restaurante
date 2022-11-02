using MongoDB.Driver;
using Restaurante.Data.Schemas;
using Restaurante.Domain.Entities;
using Restaurante.Domain.ValueObjects;
using System.Linq;
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
        public async Task<IEnumerable<Restaurant>> ObterTodos()
        {
            var restaurantes = new List<Restaurant>();
            var filter = Builders<RestaurantSchema>.Filter.Empty;
            await _restaurantes.Find(filter).ForEachAsync(d =>
            {
                var r = new Restaurant(d.Nome, d.Cozinha);
                var e = new Endereco(d.Endereco.Logradouro, d.Endereco.Cidade, d.Endereco.Cep, d.Endereco.UF, d.Endereco.Numero);
                r.Id = d.Id.ToString();
                r.AtribuirEndereco(e);
                restaurantes.Add(r);
            });
            return restaurantes;
        }

        public RestaurantSchema ObterPorId(string id)
        {
            var document = _restaurantes.AsQueryable().FirstOrDefault(_ => _.Id == id);
            if (document == null)
            {
                return null;
            }
            return document;
        }
        public RestaurantSchema ObterPorNome(string nome)
        {
            var document = _restaurantes.AsQueryable().FirstOrDefault(_ => _.Nome == nome);
            if (document == null)
            {
                return null;
            }
            return document;
        }
    }
}
