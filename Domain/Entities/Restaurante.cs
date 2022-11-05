using Restaurante.Domain.Enums;
using Restaurante.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Restaurante.Domain.Entities
{
    public class Restaurant : AbstractValidator<Restaurant>
    {
        public Restaurant(string nome, ECozinha cozinha)
        {
            Nome = nome;
            Cozinha = cozinha;
            Avaliacoes = new List<Avaliacao>();
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public ECozinha Cozinha { get; set; }
        public Endereco Endereco { get; set; }

        public ValidationResult ValidationResult { get; set; }
        public List<Avaliacao> Avaliacoes { get; set; }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public bool Validar()
        {
            ValidarNome();
            ValidationResult = Validate(this);

            ValidarEndereco();

            return ValidationResult.IsValid;
        }

        private void ValidarNome()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("Nome não pode ser vazio.")
                .MaximumLength(30).WithMessage("Nome pode ter no maximo 30 caracteres.");
        }

        private void ValidarEndereco()
        {
            if (Endereco.Validar())
                return;

            foreach (var erro in Endereco.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);
        }
        public void InserirAvaliacao(Avaliacao avaliacao)
        {
            Avaliacoes.Add(avaliacao);
        }
    }
}