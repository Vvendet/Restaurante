namespace Restaurante.Data.Schemas
{
    public class EnderecoSchema
    {
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Cidade { get; private set; }
        public string UF { get; private set; }
        public string Cep { get; private set; }
    }
}
