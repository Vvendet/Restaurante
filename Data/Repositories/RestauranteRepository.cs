using MongoDB.Driver;
using Restaurante.Data.Schemas;
using Restaurante.Domain.Entities;
using Restaurante.Domain.Enums;
using Restaurante.Domain.ValueObjects;
using System.Linq;
namespace Restaurante.Data.Repositories
{
    public class RestauranteRepository
    {
        IMongoCollection<RestaurantSchema> _restaurantes;
        IMongoCollection<AvaliacaoSchema> _avaliacoes;

        public RestauranteRepository(MongoDB mongoDB)
        {
            _restaurantes = mongoDB.DB.GetCollection<RestaurantSchema>("restaurantes");
            _avaliacoes = mongoDB.DB.GetCollection<AvaliacaoSchema>("avaliacoes");
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
        public Restaurant SchemaToNormal(RestaurantSchema restaurante)
        {
            var document = new Restaurant(restaurante.Nome, restaurante.Cozinha);
            var e = new Endereco(restaurante.Endereco.Logradouro, restaurante.Endereco.Numero, restaurante.Endereco.Cidade, restaurante.Endereco.UF, restaurante.Endereco.Cep);
            document.AtribuirEndereco(e);
            document.Id = restaurante.Id;
            return document;
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

        public bool AlterarCompleto(RestaurantSchema restaurante)
        {
            var resultado = _restaurantes.ReplaceOne(_ => _.Id == restaurante.Id, restaurante);
            return resultado.ModifiedCount > 0; 
        }
        public bool AlterarCozinha(string id, ECozinha cozinha)
        {
            var atualizacao = Builders<RestaurantSchema>.Update.Set(_ => _.Cozinha, cozinha);
            var resultado = _restaurantes.UpdateOne(_ => _.Id == id, atualizacao);
            return resultado.ModifiedCount > 0;
        }
        
        public void Avaliar(string restauranteId, Avaliacao avaliacao)
        {
            var dc = new AvaliacaoSchema
            {
                RestauranteId = restauranteId,
                Estrelas = avaliacao.Estrelas,
                Comentario = avaliacao.Comentario,
            };
            _avaliacoes.InsertOne(dc);
        }

        public async Task<IEnumerable<Avaliacao>> ObterAvaliacoesw()
        {
            var avaliacoes = new List<Avaliacao>();
            var filter = Builders<AvaliacaoSchema>.Filter.Empty;
            await _avaliacoes.Find(filter).ForEachAsync(d =>
            {
                var a = new Avaliacao(d.Estrelas, d.Comentario);
                
                avaliacoes.Add(a);
            });
            return avaliacoes;
        }

        public async Task<Dictionary<Restaurant, double>> ObterTop3()
        {
            var retorno = new Dictionary<Restaurant, double>();

            var top3 = _avaliacoes.Aggregate()
                .Group(_ => _.RestauranteId, g => new { RestauranteId = g.Key, MediaEstrelas = g.Average(a => a.Estrelas) })
                .SortByDescending(_ => _.MediaEstrelas)
                .Limit(3);

            await top3.ForEachAsync(_ =>
            {
                var restaurante = SchemaToNormal(ObterPorId(_.RestauranteId));
                _avaliacoes.AsQueryable()
                    .Where(a => a.RestauranteId == _.RestauranteId)
                    .ToList()
                    .ForEach(a => restaurante.InserirAvaliacao(a.ConverterParaDomain()));

                retorno.Add(restaurante, _.MediaEstrelas);
            });

            return retorno;
        }

        public (long, long) Remover(string restauranteId)
        {
            var resultadoAvaliacoes = _avaliacoes.DeleteMany(_ => _.RestauranteId == restauranteId);
            var resultadoRestaurante = _restaurantes.DeleteOne(_ => _.Id == restauranteId);

            return (resultadoAvaliacoes.DeletedCount, resultadoRestaurante.DeletedCount);
        }
    }
}
