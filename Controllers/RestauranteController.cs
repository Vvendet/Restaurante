using Restaurante.Controllers.Inputs;
using Microsoft.AspNetCore.Mvc;
using Restaurante.Domain.Enums;
using Restaurante.Domain.Entities;
using Restaurante.Domain.ValueObjects;

namespace Restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestauranteController : ControllerBase
    {
        
        [HttpPost("restaurante")]
        public ActionResult IncluirRestaurante([FromBody] RestauranteInclusao restauranteInclusao)
        {
            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteInclusao.Cozinha);

            var restaurante = new Restaurant(restauranteInclusao.Nome, cozinha);
            var endereco = new Endereco(
                restauranteInclusao.Logradouro,
                restauranteInclusao.Numero,
                restauranteInclusao.Cidade,
                restauranteInclusao.UF,
                restauranteInclusao.Cep);
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