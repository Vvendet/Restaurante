using Restaurante.Domain.Enums;

namespace Restaurante.Controllers.Outputs
{
    public class RestauranteTop3
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public string Cidade { get; set; }
        public string Estrelas { get; set; }
    }
}
