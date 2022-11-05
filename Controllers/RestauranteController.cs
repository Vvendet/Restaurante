using Restaurante.Controllers.Inputs;
using Microsoft.AspNetCore.Mvc;
using Restaurante.Domain.Enums;
using Restaurante.Domain.Entities;
using Restaurante.Domain.ValueObjects;
using Restaurante.Data.Repositories;
using Restaurante.Controllers.Outputs;
using Restaurante.Data.Schemas;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestauranteController : ControllerBase
    {
        private readonly RestauranteRepository _restauranteRepository;
        public RestauranteController(RestauranteRepository restauranteRepository)
        {
            _restauranteRepository = restauranteRepository;
        }
        
        [HttpPost("restaurante")]
        public ActionResult IncluirRestaurante([FromQuery] string nome, int ccozinha, string logradouro, string numero, string cidade, string uf, string cep)
        {
            var cozinha = ECozinhaHelper.ConverterDeInteiro(ccozinha);

            var restaurante = new Restaurant(nome, cozinha);
            var endereco = new Endereco(
                logradouro,
                numero,
                cidade,
                uf,
                cep);
            restaurante.AtribuirEndereco(endereco);

            if (!restaurante.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurante.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }
            _restauranteRepository.Inserir(restaurante);
            return Ok(
                new
                {
                    data = "Restaurante inserido com sucesso"
                });
        }

        [HttpGet("restaurante/todos")]
        public async Task<ActionResult> ObterRestaurantes()
        {
            var restaurantes = await _restauranteRepository.ObterTodos();

            var listagem = restaurantes.Select(_ => new RestauranteListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Cozinha = (int)_.Cozinha,
                Cidade = _.Endereco.Cidade,
            });
            return Ok(
                new
                {
                    data = listagem
                }
            );
        }

        [HttpGet("restaurante/byid/{id}")]
        public ActionResult ObterRestauranteI(string id)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);
            if (restaurante == null)
                return NotFound();
            return Ok(
                new {data = restaurante });

        }

        [HttpGet("restaurante/byname/{nome}")]
        public ActionResult ObterRestauranteN(string nome)
        {
            var restaurante = _restauranteRepository.ObterPorNome(nome);
            if (restaurante == null)
                return NotFound();
            return Ok(
                new { data = restaurante });

        }
        [HttpPut("restaurante")]
        public ActionResult AlterarRestauranteCompleto([FromQuery] string id, string nome, int ccozinha, string logradouro, string numero, string cidade, string uf, string cep)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);
            
            if (restaurante == null)
            {
                return NotFound();
            }
            var cozinha = ECozinhaHelper.ConverterDeInteiro(ccozinha);
            var restaurante2 = new Restaurant(nome, cozinha);
            var endereco = new Endereco(logradouro, numero, cidade, uf, cep);
            restaurante2.Id = id;
            restaurante2.AtribuirEndereco(endereco);
            if (!restaurante2.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurante2.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }

            var document = new RestaurantSchema()
            {
                Id = restaurante2.Id,
                Nome = restaurante2.Nome,
                Cozinha = restaurante2.Cozinha,

                Endereco = new EnderecoSchema()
                {
                    Logradouro = restaurante2.Endereco.Logradouro,
                    Numero = restaurante2.Endereco.Numero,
                    Cidade = restaurante2.Endereco.Cidade,
                    Cep = restaurante2.Endereco.Cep,
                    UF = restaurante2.Endereco.UF
                }
            };
            if (!_restauranteRepository.AlterarCompleto(document))
            {
                return BadRequest(new
                {
                    errors = "Nenhum documento foi alterado"
                });

            }
            return Ok(new
            {
                data = "Restaurante alterado com sucesso"
            });
        }

        [HttpPatch("restaurante/byid/avaliar")]
        public ActionResult AvaliarRestaurante([FromQuery] string id, int estrelas, string comentario)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);
            if (restaurante == null)
            {
                return NotFound();
            }

            var avaliacao = new Avaliacao(estrelas, comentario);

            if (!avaliacao.Validar())
            {
                return BadRequest(new
                {
                    errors = avaliacao.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                });
            }

            _restauranteRepository.Avaliar(id, avaliacao);
            return Ok(new
            {
                data = "restaurante avaliado com sucesso"
            });
            
        }

        [HttpGet("restaurante/avaliacoes")]
        public async Task<ActionResult> ObterAvaliacoes()
        {
            var avaliacoes = await _restauranteRepository.ObterAvaliacoesw();

            var listagem = avaliacoes.Select(_ => new AvaliacaoListagem
            {
                Estrelas = _.Estrelas,
                Comentario = _.Comentario
            });
            return Ok(
                new
                {
                    data = listagem
                }
            );
        }

        [HttpGet("restaurante/top3")]
        public async Task<ActionResult> ObterTop3Restaurantes()
        {
            var top3 = await _restauranteRepository.ObterTop3();
            var listagem = top3.Select(_ => new RestauranteTop3
            {
                Id = _.Key.Id,
                Nome = _.Key.Nome,
                Cozinha = _.Key.Cozinha,
                Cidade = _.Key.Endereco.Cidade,
                Estrelas = _.Value.ToString()

            });

            return Ok(new {data = listagem});

        }

        [HttpDelete("restaurante/delete/{id}")]
        public ActionResult Remover(string id)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);
            if (restaurante == null)
            {
                return NotFound();
            }
            (var totalAvaliacoesRemovidas, var totalRestauranteRemovido ) = _restauranteRepository.Remover(id);

            return Ok(new { data = $"Avaliações removidas: {totalAvaliacoesRemovidas}; Restaurantes removidos: {totalRestauranteRemovido}" });
        }

    };

        

}