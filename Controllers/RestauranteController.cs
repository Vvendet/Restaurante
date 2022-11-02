using Restaurante.Controllers.Inputs;
using Microsoft.AspNetCore.Mvc;
using Restaurante.Domain.Enums;
using Restaurante.Domain.Entities;
using Restaurante.Domain.ValueObjects;
using Restaurante.Data.Repositories;

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
        
    };

}