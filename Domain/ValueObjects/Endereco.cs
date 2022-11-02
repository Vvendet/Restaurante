using FluentValidation;
using FluentValidation.Results;

namespace Restaurante.Domain.ValueObjects
{
    public class Endereco : AbstractValidator<Endereco>
    {
        public Endereco(string logradouro, string numero, string cidade, string uf, string cep)
        {
            Logradouro = logradouro;
            Numero = numero;
            Cidade = cidade;
            UF = uf;
            Cep = cep;
        }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Cidade { get; private set; }
        public string UF { get; private set; }
        public string Cep { get; private set; }
        public ValidationResult ValidationResult { get; set; }


        public bool Validar()
        {
            ValidarLogradouro();
            ValidarCidade();
            ValidarUF();
            ValidarCep();

            ValidationResult = Validate(this);
            return ValidationResult.IsValid;
        }
        public void ValidarLogradouro()
        {
            RuleFor(c => c.Logradouro)
                .NotEmpty().WithMessage("Logradouro não pode ser vazio")
                .MaximumLength(50).WithMessage("Logradouro deve ter no máximo 50 caracteres");
        }
        public void ValidarCidade()
        {
            RuleFor(c => c.Cidade)
                .NotEmpty().WithMessage("Cidade não pode ser vazio")
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");
        }
        public void ValidarUF()
        {
            RuleFor(c => c.UF)
                .NotEmpty().WithMessage("UF não pode ser vazio")
                .Length(2).WithMessage("UF deve ter 2 caracteres");
        }
        public void ValidarCep()
        {
            RuleFor(c => c.Cep)
                .NotEmpty().WithMessage("Cep não pode ser vazio")
                .Length(8).WithMessage("Cep deve ter 8 caracteres");
        }
    }
}
